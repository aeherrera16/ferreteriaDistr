// Rutas de artículos - limpio, comentado y alineado a SOLID
import pool from '../db.js'; // Conexión a la base de datos
import { Router } from 'express'; // Router de Express
import { articuloSchema } from '../validators.js'; // Esquema de validación Joi
import { insertarArticulo, consultarPorCodigo } from '../soapClient.js'; // Funciones SOAP
import { actualizarArticulo } from '../soapClient.js';

const router = Router(); // Instancia de router

// Middleware de autenticación para proteger rutas
function ensureAuth(req, res, next) {
  if (req.session?.auth) return next();
  return res.redirect('/login');
}

// Obtener todas las categorías (para selects)
router.get('/api/categorias', ensureAuth, async (req, res) => {
  try {
    const result = await pool.query('SELECT id, nombre FROM categorias ORDER BY nombre');
    res.json(result.rows);
  } catch (e) {
    // Log de error
    console.error('Error al obtener categorías:', e);
    res.status(500).json({ error: 'Error al obtener categorías' });
  }
});

// Listar todos los artículos (vista)
router.get('/', ensureAuth, async (req, res) => {
  try {
    const result = await pool.query(`
      SELECT a.id, a.codigo, a.nombre, a.stock, a.stockminimo, a.precioventa, a.preciocompra, c.nombre AS categoria, p.nombre AS proveedor
      FROM articulos a
      LEFT JOIN categorias c ON a.categoriaid = c.id
      LEFT JOIN proveedores p ON a.proveedorid = p.id
      WHERE a.activo = true
      ORDER BY a.nombre
    `);
  res.render('articulos', { articulos: result.rows, error: null });
  } catch (e) {
    console.error('Error al listar articulos:', e);
    res.render('articulos', { articulos: [], error: 'Error al obtener artículos' });
  }
});

// Obtener todos los proveedores (para selects)
router.get('/api/proveedores', ensureAuth, async (req, res) => {
  try {
    const result = await pool.query('SELECT id, nombre FROM proveedores ORDER BY nombre');
    res.json(result.rows);
  } catch (e) {
    // Log de error
    console.error('Error al obtener proveedores:', e);
    res.status(500).json({ error: 'Error al obtener proveedores' });
  }
});


// Renderizar formulario de creación de artículo
router.get('/crear', ensureAuth, (req, res) => {
  res.render('crear', { data: {}, errors: {}, serverMsg: null });
});

// Renderizar formulario de edición
router.get('/:id/editar', ensureAuth, async (req, res) => {
  const id = Number(req.params.id);
  if (!id) return res.redirect('/articulos');
  try {
    const result = await pool.query('SELECT id, codigo, nombre, descripcion, categoriaid AS categoria, preciocompra AS "PrecioCompra", precioventa AS "PrecioVenta", stock, stockminimo AS "StockMinimo", proveedorid AS proveedor FROM articulos WHERE id = $1', [id]);
    if (result.rowCount === 0) return res.redirect('/articulos');
    // Mapear resultado a la estructura que espera la vista
    const row = result.rows[0];
    const data = {
      Codigo: row.codigo,
      Nombre: row.nombre,
      Descripcion: row.descripcion,
      Categoria: row.categoria || '',
      PrecioCompra: row.PrecioCompra || 0,
      PrecioVenta: row.PrecioVenta || 0,
      Stock: row.stock || 0,
      StockMinimo: row.StockMinimo || 0,
      Proveedor: row.proveedor || ''
    };
    res.render('editar', { id, data, errors: {}, serverMsg: null });
  } catch (e) {
    console.error('Error al obtener artículo para edición:', e);
    res.redirect('/articulos');
  }
});

// Procesar creación de artículo
router.post('/crear', ensureAuth, async (req, res) => {
  // Construir el payload con los datos del formulario
  const payload = {
    Codigo: req.body.codigo?.trim(),
    Nombre: req.body.nombre?.trim(),
    Categoria: req.body.categoria?.trim() || '',
    PrecioCompra: Number(req.body.precio_compra),
    PrecioVenta: Number(req.body.precio_venta),
    Stock: Number(req.body.stock),
    StockMinimo: Number(req.body.stock_minimo),
    Proveedor: req.body.proveedor?.trim() || ''
  };

  // Validación de datos con Joi
  const { error } = articuloSchema.validate(payload, { abortEarly: false });
  if (error) {
    // Mapear errores para mostrar en la UI
    const errors = Object.fromEntries(error.details.map(d => [d.context.key, d.message]));
    // Mensaje descriptivo para el usuario
    return res.render('crear', { data: payload, errors, serverMsg: 'Corrija los errores del formulario.' });
  }

  // Verificar sesión y token
  const token = req.session?.token;
  if (!token) {
    return res.render('crear', { data: payload, errors: {}, serverMsg: 'No autenticado contra el servicio SOAP. Inicie sesión primero.' });
  }

  // Verificar unicidad del código en la base de datos local antes de intentar insertar
  try {
    const exists = await pool.query('SELECT 1 FROM articulos WHERE codigo = $1', [payload.Codigo]);
    if (exists.rowCount > 0) {
      return res.render('crear', { data: payload, errors: { Codigo: 'El código ya existe en la base de datos.' }, serverMsg: 'Código duplicado. Use otro código.' });
    }
  } catch (e) {
    console.error('Error comprobando unicidad de código:', e);
    // No bloqueamos al usuario por este fallo puntual; continuamos intentando insertar vía SOAP
  }

  try {
    // Intentar insertar el artículo vía SOAP
    const result = await insertarArticulo(payload, token);
    const msg = result?.Mensaje || (result?.Exito ? 'Insertado' : 'Error al insertar');
    res.render('crear', { data: {}, errors: {}, serverMsg: msg });
  } catch (e) {
    // Log de error para depuración
    console.error('Error al insertar artículo:', e);
    // Mensaje amigable para el usuario
    res.render('crear', { data: payload, errors: {}, serverMsg: 'Ocurrió un error inesperado al registrar el artículo. Intente más tarde.' });
  }
});

// Procesar actualización de artículo
router.post('/:id/editar', ensureAuth, async (req, res) => {
  const id = Number(req.params.id);
  if (!id) return res.redirect('/articulos');

  const payload = {
    Codigo: req.body.codigo?.trim(),
    Nombre: req.body.nombre?.trim(),
    Categoria: req.body.categoria?.trim() || '',
    PrecioCompra: Number(req.body.precio_compra),
    PrecioVenta: Number(req.body.precio_venta),
    Stock: Number(req.body.stock),
    StockMinimo: Number(req.body.stock_minimo),
    Proveedor: req.body.proveedor?.trim() || ''
  };

  // Validación
  const { error } = articuloSchema.validate(payload, { abortEarly: false });
  if (error) {
    const errors = Object.fromEntries(error.details.map(d => [d.context.key, d.message]));
    return res.render('editar', { id, data: payload, errors, serverMsg: 'Corrija los errores del formulario.' });
  }

  // Verificar unicidad del código para otros registros
  try {
    const exists = await pool.query('SELECT id FROM articulos WHERE codigo = $1 AND id != $2', [payload.Codigo, id]);
    if (exists.rowCount > 0) {
      return res.render('editar', { id, data: payload, errors: { Codigo: 'El código ya existe en otro artículo.' }, serverMsg: 'Código duplicado. Use otro código.' });
    }
  } catch (e) {
    console.error('Error comprobando unicidad antes de actualizar:', e);
  }

  const token = req.session?.token;
  if (!token) {
    return res.render('editar', { id, data: payload, errors: {}, serverMsg: 'No autenticado contra el servicio SOAP. Inicie sesión primero.' });
  }

  try {
    const result = await actualizarArticulo(id, payload, token);
    const msg = result?.Mensaje || (result?.Exito ? 'Actualizado' : 'Error al actualizar');
    // Si la actualización fue exitosa, redirigimos al listado
    if (result?.Exito) return res.redirect('/articulos');
    return res.render('editar', { id, data: payload, errors: {}, serverMsg: msg });
  } catch (e) {
    console.error('Error al actualizar artículo:', e);
    return res.render('editar', { id, data: payload, errors: {}, serverMsg: 'Ocurrió un error inesperado al actualizar el artículo. Intente más tarde.' });
  }
});


// Renderizar formulario de consulta
router.get('/consultar', ensureAuth, (req, res) => {
  res.render('consultar', { articulo: null, error: null });
});

// Procesar consulta de artículo por código

import { consultarPorNombre } from '../soapClient.js';

// Permitir consulta por código o por nombre
router.post('/consultar', ensureAuth, async (req, res) => {
  const tipoBusqueda = req.body.tipoBusqueda;
  const codigo = (req.body.codigo || '').trim();
  const nombre = (req.body.nombre || '').trim();
  if ((tipoBusqueda === 'codigo' && !codigo) || (tipoBusqueda === 'nombre' && !nombre)) {
    return res.render('consultar', { articulo: null, error: 'Ingrese un ' + (tipoBusqueda === 'codigo' ? 'código' : 'nombre') + ' para consultar.' });
  }
  const token = req.session?.token;
  if (!token) {
    return res.render('consultar', { articulo: null, error: 'No autenticado contra el servicio SOAP. Inicie sesión primero.' });
  }
  try {
    let art = null;
    if (tipoBusqueda === 'codigo') {
      // Intentar obtener por SOAP primero
      try {
        art = await consultarPorCodigo(codigo, token);
      } catch (soapErr) {
        console.error('SOAP error al consultar por código, intentando BD:', soapErr);
        art = null;
      }
      // Si no se obtuvo resultado vía SOAP, intentar consulta directa a la BD
      if (!art) {
        const q = await pool.query(`
          SELECT a.codigo AS "Codigo", a.nombre AS "Nombre", a.descripcion AS "Descripcion", a.preciocompra AS "PrecioCompra", a.precioventa AS "PrecioVenta", a.stock AS "Stock", a.stockminimo AS "StockMinimo", c.nombre AS "Categoria", p.nombre AS "Proveedor"
          FROM articulos a
          LEFT JOIN categorias c ON a.categoriaid = c.id
          LEFT JOIN proveedores p ON a.proveedorid = p.id
          WHERE a.codigo = $1
          LIMIT 1
        `, [codigo]);
        if (q.rowCount > 0) art = q.rows[0];
      }
    } else if (tipoBusqueda === 'nombre') {
      try {
        art = await consultarPorNombre(nombre, token);
      } catch (soapErr) {
        console.error('SOAP error al consultar por nombre, intentando BD:', soapErr);
        art = null;
      }
      // Si SOAP no devolvió resultado, buscar en BD por coincidencia parcial (ILIKE)
      if (!art) {
        const q = await pool.query(`
          SELECT a.codigo AS "Codigo", a.nombre AS "Nombre", a.descripcion AS "Descripcion", a.preciocompra AS "PrecioCompra", a.precioventa AS "PrecioVenta", a.stock AS "Stock", a.stockminimo AS "StockMinimo", c.nombre AS "Categoria", p.nombre AS "Proveedor"
          FROM articulos a
          LEFT JOIN categorias c ON a.categoriaid = c.id
          LEFT JOIN proveedores p ON a.proveedorid = p.id
          WHERE a.nombre ILIKE '%' || $1 || '%'
          ORDER BY a.nombre
          LIMIT 1
        `, [nombre]);
        if (q.rowCount > 0) art = q.rows[0];
      }
    }
    if (!art) {
      return res.render('consultar', { articulo: null, error: 'No se encontró el artículo.' });
    }
    // Normalizar claves: si el resultado viene desde SOAP podría tener mayúsculas/minúsculas distintas
    const normalize = (a) => {
      if (!a) return a;
      // Si ya tiene Stock en mayúscula, devolver tal cual
      if (a.Stock !== undefined || a.StockMinimo !== undefined) return a;
      // Mapear nombres comunes a la vista
      return {
        Stock: a.stock ?? a.Stock ?? 0,
        StockMinimo: a.stockminimo ?? a.StockMinimo ?? 0,
        Codigo: a.Codigo ?? a.codigo ?? a.cod ?? '',
        Nombre: a.Nombre ?? a.nombre ?? '',
        Descripcion: a.Descripcion ?? a.descripcion ?? '',
        PrecioCompra: a.PrecioCompra ?? a.preciocompra ?? a.precio_compra ?? 0,
        PrecioVenta: a.PrecioVenta ?? a.precioventa ?? a.precio_venta ?? 0,
        Categoria: a.Categoria ?? a.categoria ?? a.categorianombre ?? null,
        Proveedor: a.Proveedor ?? a.proveedor ?? a.proveedornombre ?? null
      };
    };
    res.render('consultar', { articulo: normalize(art), error: null });
  } catch (e) {
    // Log de error para depuración
    console.error('Error al consultar artículo:', e);
    res.render('consultar', { articulo: null, error: 'Ocurrió un error inesperado al consultar el artículo. Intente más tarde.' });
  }
});

export default router;



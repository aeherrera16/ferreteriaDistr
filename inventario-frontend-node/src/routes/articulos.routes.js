// Rutas de artículos - limpio, comentado y alineado a SOLID
import pool from '../db.js'; // Conexión a la base de datos
import { Router } from 'express'; // Router de Express
import { articuloSchema } from '../validators.js'; // Esquema de validación Joi
import { insertarArticulo, consultarPorCodigo } from '../soapClient.js'; // Funciones SOAP

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
      art = await consultarPorCodigo(codigo, token);
    } else if (tipoBusqueda === 'nombre') {
      art = await consultarPorNombre(nombre, token);
    }
    res.render('consultar', { articulo: art || null, error: art ? null : 'No se encontró el artículo.' });
  } catch (e) {
    // Log de error para depuración
    console.error('Error al consultar artículo:', e);
    res.render('consultar', { articulo: null, error: 'Ocurrió un error inesperado al consultar el artículo. Intente más tarde.' });
  }
});

export default router;



import { Router } from 'express';
import { articuloSchema } from '../validators.js';
import { insertarArticulo, consultarPorCodigo } from '../soapClient.js';

const router = Router();

function ensureAuth(req, res, next) {
  if (req.session?.auth) return next();
  return res.redirect('/login');
}

router.get('/crear', ensureAuth, (req, res) => {
  res.render('crear', { data: {}, errors: {}, serverMsg: null });
});

router.post('/crear', ensureAuth, async (req, res) => {
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

  const { error } = articuloSchema.validate(payload, { abortEarly: false });
  if (error) {
    const errors = Object.fromEntries(error.details.map(d => [d.context.key, d.message]));
    return res.render('crear', { data: payload, errors, serverMsg: null });
  }

  try {
    const result = await insertarArticulo(payload);
    const msg = result?.Mensaje || (result?.Exito ? 'Insertado' : 'Error al insertar');
    res.render('crear', { data: {}, errors: {}, serverMsg: msg });
  } catch (e) {
    res.render('crear', { data: payload, errors: {}, serverMsg: `SOAP Fault: ${e.message}` });
  }
});

router.get('/consultar', ensureAuth, (req, res) => {
  res.render('consultar', { articulo: null, error: null });
});

router.post('/consultar', ensureAuth, async (req, res) => {
  const codigo = (req.body.codigo || '').trim();
  if (!codigo) return res.render('consultar', { articulo: null, error: 'Ingrese un código' });
  try {
    const art = await consultarPorCodigo(codigo);
    res.render('consultar', { articulo: art || null, error: art ? null : 'No encontrado' });
  } catch (e) {
    res.render('consultar', { articulo: null, error: `SOAP Fault: ${e.message}` });
  }
});

export default router;

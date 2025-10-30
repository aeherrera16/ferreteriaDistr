import express from 'express';
import pool from '../db.js';
const router = express.Router();
// Usar BD directamente para proveedores

// Listar proveedores
router.get('/', async (req, res) => {
  try {
    if (!req.session?.auth) return res.redirect('/login');
    const result = await pool.query('SELECT id, nombre, telefono, email, direccion FROM proveedores ORDER BY nombre');
  res.render('proveedores', { proveedores: result.rows, error: null });
  } catch (err) {
    console.error('Error al obtener proveedores desde BD:', err);
    res.render('proveedores', { proveedores: [], error: 'Error al obtener proveedores' });
  }
});

// Formulario crear proveedor
router.get('/crear', (req, res) => {
  res.render('crearProveedor', { error: null });
});

// Crear proveedor
router.post('/crear', async (req, res) => {
  try {
    await soapClient.crearProveedor(req.body);
    res.redirect('/proveedores');
  } catch (err) {
    res.render('crearProveedor', { error: err.message });
  }
});

export default router;

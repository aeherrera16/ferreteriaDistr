import express from 'express';
import pool from '../db.js';
const router = express.Router();
// Usar la base de datos directamente para mostrar categorías

// Listar categorías
router.get('/', async (req, res) => {
  try {
    if (!req.session?.auth) return res.redirect('/login');
    const result = await pool.query('SELECT id, nombre, descripcion FROM categorias ORDER BY nombre');
  res.render('categorias', { categorias: result.rows, error: null });
  } catch (err) {
    console.error('Error al obtener categorías desde BD:', err);
    res.render('categorias', { categorias: [], error: 'Error al obtener categorías' });
  }
});

// Formulario crear categoría
router.get('/crear', (req, res) => {
  res.render('crearCategoria', { error: null });
});

// Crear categoría
router.post('/crear', async (req, res) => {
  try {
    await soapClient.crearCategoria(req.body);
    res.redirect('/categorias');
  } catch (err) {
    res.render('crearCategoria', { error: err.message });
  }
});

export default router;

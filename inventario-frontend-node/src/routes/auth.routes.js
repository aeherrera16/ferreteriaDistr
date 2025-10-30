
import { Router } from 'express';

import pool from '../db.js';
import { autenticarSoap } from '../soapClient.js';

const router = Router();

router.get('/login', (req, res) => {
  res.render('login', { error: null });
});

router.post('/login', async (req, res) => {
  const { user, pass } = req.body;
  try {
    // Validar usuario en la base de datos
    const result = await pool.query(
      'SELECT * FROM usuarios WHERE nombreusuario = $1 AND passwordhash = $2 AND activo = true',
      [user, pass]
    );
    if (result.rows.length > 0) {
      // Autenticar contra el servicio SOAP y guardar el token
      try {
        const token = await autenticarSoap(user, pass);
        req.session.auth = true;
        req.session.user = {
          id: result.rows[0].id,
          nombreusuario: result.rows[0].nombreusuario,
          rol: result.rows[0].rol
        };
  req.session.token = token;
  // Redirigir al dashboard principal después del login
  return res.redirect('/');
      } catch (e) {
        // Mostrar error completo en pantalla y consola
        console.error('Error autenticando contra el servicio SOAP:', e);
        return res.render('login', { error: 'Error autenticando contra el servicio SOAP: ' + (e && e.message ? e.message : JSON.stringify(e)) });
      }
    } else {
      return res.render('login', { error: 'Credenciales inválidas' });
    }
  } catch (e) {
    return res.render('login', { error: 'Error de conexión a la base de datos' });
  }
});

router.get('/logout', (req, res) => {
  req.session.destroy(() => res.redirect('/login'));
});

export default router;

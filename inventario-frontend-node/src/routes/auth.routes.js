import { Router } from 'express';

const router = Router();

router.get('/login', (req, res) => {
  res.render('login', { error: null });
});

router.post('/login', (req, res) => {
  const { user, pass } = req.body;
  if (user === process.env.ADMIN_USER && pass === process.env.ADMIN_PASS) {
    req.session.auth = true;
    return res.redirect('/articulos/crear');
  }
  return res.render('login', { error: 'Credenciales inválidas' });
});

router.get('/logout', (req, res) => {
  req.session.destroy(() => res.redirect('/login'));
});

export default router;

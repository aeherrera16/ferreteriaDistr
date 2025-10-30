import express from 'express';
import session from 'express-session';
import path from 'path';
import dotenv from 'dotenv';
import bodyParser from 'body-parser';
import expressLayouts from 'express-ejs-layouts';
import authRoutes from './routes/auth.routes.js';
import artRoutes from './routes/articulos.routes.js';
import categoriasRoutes from './routes/categorias.routes.js';
import proveedoresRoutes from './routes/proveedores.routes.js';

dotenv.config();
const app = express();

app.set('view engine', 'ejs');
app.set('views', path.join(process.cwd(), 'src', 'views'));
app.use(express.static(path.join(process.cwd(), 'public')));
app.use(expressLayouts);
app.set('layout', 'layout');

// En desarrollo, añadir una CSP permisiva para permitir fuentes externas y conexiones al backend local
app.use((req, res, next) => {
  // Ajusta estas directivas según políticas de seguridad del despliegue
  res.setHeader('Content-Security-Policy', "default-src 'self' 'unsafe-inline' data: https:; font-src 'self' https://fonts.gstatic.com; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com https:; connect-src 'self' http://localhost:5233 http://localhost:3000;");
  next();
});

app.use(bodyParser.urlencoded({ extended: true }));
app.use(session({
  secret: process.env.SESSION_SECRET,
  resave: false,
  saveUninitialized: false
}));
 
// Exponer la ruta actual a las vistas para renderizado condicional (ej. mostrar botones por página)
app.use((req, res, next) => {
  res.locals.currentPath = req.path || req.originalUrl;
  next();
});

// Middleware para proteger rutas (excepto login)
app.use((req, res, next) => {
  const openPaths = ['/login', '/logout'];
  if (openPaths.includes(req.path) || req.path.startsWith('/public') || req.path.startsWith('/css')) {
    return next();
  }
  if (!req.session.auth) {
    return res.redirect('/login');
  }
  next();
});

app.use('/', authRoutes);
app.use('/articulos', artRoutes);
app.use('/categorias', categoriasRoutes);
app.use('/proveedores', proveedoresRoutes);


app.get('/', (req, res) => {
  if (!req.session.auth) return res.redirect('/login');
  res.render('home');
});

const port = process.env.PORT || 3000;
app.listen(port, () => {
  console.log(`UI Node escuchando en http://localhost:${port}`);
});

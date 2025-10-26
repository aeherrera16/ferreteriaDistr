import express from 'express';
import session from 'express-session';
import path from 'path';
import dotenv from 'dotenv';
import bodyParser from 'body-parser';
import expressLayouts from 'express-ejs-layouts';
import authRoutes from './routes/auth.routes.js';
import artRoutes from './routes/articulos.routes.js';

dotenv.config();
const app = express();

app.set('view engine', 'ejs');
app.set('views', path.join(process.cwd(), 'src', 'views'));
app.use(express.static(path.join(process.cwd(), 'public')));
app.use(expressLayouts);

app.use(bodyParser.urlencoded({ extended: true }));
app.use(session({
  secret: process.env.SESSION_SECRET,
  resave: false,
  saveUninitialized: false
}));

app.use('/', authRoutes);
app.use('/articulos', artRoutes);

app.get('/', (req, res) => res.redirect('/login'));

const port = process.env.PORT || 3000;
app.listen(port, () => {
  console.log(`UI Node escuchando en http://localhost:${port}`);
});

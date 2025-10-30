--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5
-- Dumped by pg_dump version 17.5

-- Started on 2025-10-25 16:19:19

SET statement_timeout = 0;

SET lock_timeout = 0;

SET idle_in_transaction_session_timeout = 0;

SET transaction_timeout = 0;

SET client_encoding = 'UTF8';

SET standard_conforming_strings = on;

SELECT pg_catalog.set_config ('search_path', '', false);

SET check_function_bodies = false;

SET xmloption = content;

SET client_min_messages = warning;

SET row_security = off;

--
-- TOC entry 227 (class 1255 OID 68002)
-- Name: actualizar_fecha_modificacion(); Type: FUNCTION; Schema: public; Owner: postgres
--

CREATE FUNCTION public.actualizar_fecha_modificacion() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    NEW.FechaActualizacion = CURRENT_TIMESTAMP;
    RETURN NEW;
END;
$$;

ALTER FUNCTION public.actualizar_fecha_modificacion() OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 224 (class 1259 OID 67954)
-- Name: articulos; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.articulos (
    id integer NOT NULL,
    codigo character varying(50) NOT NULL,
    nombre character varying(200) NOT NULL,
    descripcion text,
    categoriaid integer,
    preciocompra numeric(10, 2) NOT NULL,
    precioventa numeric(10, 2) NOT NULL,
    stock integer DEFAULT 0 NOT NULL,
    stockminimo integer DEFAULT 5 NOT NULL,
    proveedorid integer,
    fechacreacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    fechaactualizacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    activo boolean DEFAULT true,
    CONSTRAINT articulos_preciocompra_check CHECK ((preciocompra > (0)::numeric)),
    CONSTRAINT articulos_precioventa_check CHECK ((precioventa > (0)::numeric)),
    CONSTRAINT articulos_stock_check CHECK ((stock >= 0)),
    CONSTRAINT chk_precios CHECK ((precioventa >= preciocompra))
);

ALTER TABLE public.articulos OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 67953)
-- Name: articulos_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.articulos_id_seq AS integer START
WITH
    1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;

ALTER SEQUENCE public.articulos_id_seq OWNER TO postgres;

--
-- TOC entry 4916 (class 0 OID 0)
-- Dependencies: 223
-- Name: articulos_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.articulos_id_seq OWNED BY public.articulos.id;

--
-- TOC entry 220 (class 1259 OID 67936)
-- Name: categorias; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.categorias (
    id integer NOT NULL,
    nombre character varying(100) NOT NULL,
    descripcion text
);

ALTER TABLE public.categorias OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 67935)
-- Name: categorias_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.categorias_id_seq AS integer START
WITH
    1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;

ALTER SEQUENCE public.categorias_id_seq OWNER TO postgres;

--
-- TOC entry 4917 (class 0 OID 0)
-- Dependencies: 219
-- Name: categorias_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.categorias_id_seq OWNED BY public.categorias.id;

--
-- TOC entry 226 (class 1259 OID 67984)
-- Name: logoperaciones; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.logoperaciones (
    id integer NOT NULL,
    operacion character varying(50) NOT NULL,
    entidad character varying(50) NOT NULL,
    entidadid integer,
    usuarioid integer,
    detalles text,
    fechahora timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE public.logoperaciones OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 67983)
-- Name: logoperaciones_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.logoperaciones_id_seq AS integer START
WITH
    1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;

ALTER SEQUENCE public.logoperaciones_id_seq OWNER TO postgres;

--
-- TOC entry 4918 (class 0 OID 0)
-- Dependencies: 225
-- Name: logoperaciones_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.logoperaciones_id_seq OWNED BY public.logoperaciones.id;

--
-- TOC entry 222 (class 1259 OID 67945)
-- Name: proveedores; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.proveedores (
    id integer NOT NULL,
    nombre character varying(150) NOT NULL,
    telefono character varying(20),
    email character varying(100),
    direccion text
);

ALTER TABLE public.proveedores OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 67944)
-- Name: proveedores_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.proveedores_id_seq AS integer START
WITH
    1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;

ALTER SEQUENCE public.proveedores_id_seq OWNER TO postgres;

--
-- TOC entry 4919 (class 0 OID 0)
-- Dependencies: 221
-- Name: proveedores_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.proveedores_id_seq OWNED BY public.proveedores.id;

--
-- TOC entry 218 (class 1259 OID 67924)
-- Name: usuarios; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.usuarios (
    id integer NOT NULL,
    nombreusuario character varying(50) NOT NULL,
    passwordhash character varying(255) NOT NULL,
    rol character varying(20) DEFAULT 'Usuario'::character varying NOT NULL,
    fechacreacion timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    activo boolean DEFAULT true
);

ALTER TABLE public.usuarios OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 67923)
-- Name: usuarios_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.usuarios_id_seq AS integer START
WITH
    1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE CACHE 1;

ALTER SEQUENCE public.usuarios_id_seq OWNER TO postgres;

--
-- TOC entry 4920 (class 0 OID 0)
-- Dependencies: 217
-- Name: usuarios_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.usuarios_id_seq OWNED BY public.usuarios.id;

--
-- TOC entry 4722 (class 2604 OID 67957)
-- Name: articulos id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.articulos
ALTER COLUMN id
SET DEFAULT nextval(
    'public.articulos_id_seq'::regclass
);

--
-- TOC entry 4720 (class 2604 OID 67939)
-- Name: categorias id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categorias
ALTER COLUMN id
SET DEFAULT nextval(
    'public.categorias_id_seq'::regclass
);

--
-- TOC entry 4728 (class 2604 OID 67987)
-- Name: logoperaciones id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.logoperaciones
ALTER COLUMN id
SET DEFAULT nextval(
    'public.logoperaciones_id_seq'::regclass
);

--
-- TOC entry 4721 (class 2604 OID 67948)
-- Name: proveedores id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.proveedores
ALTER COLUMN id
SET DEFAULT nextval(
    'public.proveedores_id_seq'::regclass
);

--
-- TOC entry 4716 (class 2604 OID 67927)
-- Name: usuarios id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
ALTER COLUMN id
SET DEFAULT nextval(
    'public.usuarios_id_seq'::regclass
);

--
-- TOC entry 4908 (class 0 OID 67954)
-- Dependencies: 224
-- Data for Name: articulos; Type: TABLE DATA; Schema: public; Owner: postgres
--


COPY public.articulos (id, codigo, nombre, descripcion, categoriaid, preciocompra, precioventa, stock, stockminimo, proveedorid, fechacreacion, fechaactualizacion, activo) FROM stdin;
1	MART-001	Martillo de Uña 16oz	Martillo con mango de fibra de vidrio	1	8.50	15.00	25	10	1	2025-10-25 19:34:43.543288	2025-10-25 19:34:43.543288	t
2	TALAD-001	Taladro Eléctrico 500W	Taladro percutor con maletín	2	45.00	89.99	15	5	3	2025-10-25 19:34:43.543288	2025-10-25 19:34:43.543288	t
3	PINT-001	Pintura Látex Blanco 1GL	Pintura látex interior/exterior	4	12.00	22.50	50	15	2	2025-10-25 19:34:43.543288	2025-10-25 19:34:43.543288	t
4	DEST-001	Destornillador Phillips #2	Destornillador magnético	1	2.50	5.00	100	20	1	2025-10-25 19:34:43.543288	2025-10-25 19:34:43.543288	t
5	CABLE-001	Cable THW #12 AWG	Cable eléctrico calibre 12 (metro)	6	0.80	1.50	500	100	2	2025-10-25 19:34:43.543288	2025-10-25 19:34:43.543288	t
6	TUBO-001	Tubo PVC 1/2" x 3m	Tubo para agua fría	5	3.20	6.00	80	25	1	2025-10-25 19:34:43.543288	2025-10-25 19:34:43.543288	t
7	LIJA-001	Lijadora Orbital 200W	Lijadora eléctrica con bolsa recolectora	2	35.00	69.99	8	5	3	2025-10-25 19:34:43.543288	2025-10-25 19:34:43.543288	t
8	CERROJO-001	Cerrojo de Seguridad 6"	Cerrojo con llave dorado	1	8.00	15.99	30	10	2	2025-10-25 19:34:43.543288	2025-10-25 19:34:43.543288	t
9	123df	Oscar	hola	1	1.00	12.00	12	4	1	2025-10-25 20:57:37.461928	2025-10-25 20:57:37.46198	t
10	12	12	12	\N	12.00	14.00	12	1	\N	2025-10-25 21:00:35.505019	2025-10-25 21:00:35.505019	t
11	ASD123	PALA	PALA	2	10.00	20.00	100	10	2	2025-10-25 21:10:54.69605	2025-10-25 21:10:54.696102	t
12	sdsdf	sdfdf	adsdas	1	12.00	15.00	100	20	1	2025-10-25 21:13:47.651233	2025-10-25 21:13:47.651234	t
\.

--
-- TOC entry 4904 (class 0 OID 67936)
-- Dependencies: 220
-- Data for Name: categorias; Type: TABLE DATA; Schema: public; Owner: postgres
--


COPY public.categorias (id, nombre, descripcion) FROM stdin;
1	Herramientas Manuales	Martillos, destornilladores, llaves, etc.
2	Herramientas Eléctricas	Taladros, lijadoras, sierras eléctricas
3	Materiales de Construcción	Cemento, arena, ladrillos
4	Pintura	Pinturas, brochas, rodillos
5	Plomería	Tubos, llaves, accesorios de baño
6	Electricidad	Cables, interruptores, tomacorrientes
\.

--
-- TOC entry 4910 (class 0 OID 67984)
-- Dependencies: 226
-- Data for Name: logoperaciones; Type: TABLE DATA; Schema: public; Owner: postgres
--


COPY public.logoperaciones (id, operacion, entidad, entidadid, usuarioid, detalles, fechahora) FROM stdin;
\.

--
-- TOC entry 4906 (class 0 OID 67945)
-- Dependencies: 222
-- Data for Name: proveedores; Type: TABLE DATA; Schema: public; Owner: postgres
--


COPY public.proveedores (id, nombre, telefono, email, direccion) FROM stdin;
1	Ferretería Total S.A.	555-1234	ventas@ferreteriatotal.com	Av. Principal 123
2	Distribuidora El Tornillo	555-5678	info@eltornillo.com	Calle Industrial 456
3	Importadora HerramientasPlus	555-9012	contacto@herramientasplus.com	Zona Industrial Lote 10
\.

--
-- TOC entry 4902 (class 0 OID 67924)
-- Dependencies: 218
-- Data for Name: usuarios; Type: TABLE DATA; Schema: public; Owner: postgres
--


COPY public.usuarios (id, nombreusuario, passwordhash, rol, fechacreacion, activo) FROM stdin;
1	admin	Admin123!	Administrador	2025-10-25 19:34:43.543288	t
2	usuario	Usuario123!	Usuario	2025-10-25 19:34:43.543288	t
\.

--
-- TOC entry 4921 (class 0 OID 0)
-- Dependencies: 223
-- Name: articulos_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval ( 'public.articulos_id_seq', 12, true );

--
-- TOC entry 4922 (class 0 OID 0)
-- Dependencies: 219
-- Name: categorias_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval ( 'public.categorias_id_seq', 6, true );

--
-- TOC entry 4923 (class 0 OID 0)
-- Dependencies: 225
-- Name: logoperaciones_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval ( 'public.logoperaciones_id_seq', 1, false );

--
-- TOC entry 4924 (class 0 OID 0)
-- Dependencies: 221
-- Name: proveedores_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval ( 'public.proveedores_id_seq', 3, true );

--
-- TOC entry 4925 (class 0 OID 0)
-- Dependencies: 217
-- Name: usuarios_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval ( 'public.usuarios_id_seq', 2, true );

--
-- TOC entry 4743 (class 2606 OID 67972)
-- Name: articulos articulos_codigo_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.articulos
ADD CONSTRAINT articulos_codigo_key UNIQUE (codigo);

--
-- TOC entry 4745 (class 2606 OID 67970)
-- Name: articulos articulos_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.articulos
ADD CONSTRAINT articulos_pkey PRIMARY KEY (id);

--
-- TOC entry 4739 (class 2606 OID 67943)
-- Name: categorias categorias_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categorias
ADD CONSTRAINT categorias_pkey PRIMARY KEY (id);

--
-- TOC entry 4751 (class 2606 OID 67992)
-- Name: logoperaciones logoperaciones_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.logoperaciones
ADD CONSTRAINT logoperaciones_pkey PRIMARY KEY (id);

--
-- TOC entry 4741 (class 2606 OID 67952)
-- Name: proveedores proveedores_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.proveedores
ADD CONSTRAINT proveedores_pkey PRIMARY KEY (id);

--
-- TOC entry 4735 (class 2606 OID 67934)
-- Name: usuarios usuarios_nombreusuario_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
ADD CONSTRAINT usuarios_nombreusuario_key UNIQUE (nombreusuario);

--
-- TOC entry 4737 (class 2606 OID 67932)
-- Name: usuarios usuarios_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuarios
ADD CONSTRAINT usuarios_pkey PRIMARY KEY (id);

--
-- TOC entry 4746 (class 1259 OID 68001)
-- Name: idx_articulos_activo; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_articulos_activo ON public.articulos USING btree (activo);

--
-- TOC entry 4747 (class 1259 OID 68000)
-- Name: idx_articulos_categoria; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_articulos_categoria ON public.articulos USING btree (categoriaid);

--
-- TOC entry 4748 (class 1259 OID 67998)
-- Name: idx_articulos_codigo; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_articulos_codigo ON public.articulos USING btree (codigo);

--
-- TOC entry 4749 (class 1259 OID 67999)
-- Name: idx_articulos_nombre; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_articulos_nombre ON public.articulos USING btree (nombre);

--
-- TOC entry 4755 (class 2620 OID 68003)
-- Name: articulos trg_articulos_fecha_actualizacion; Type: TRIGGER; Schema: public; Owner: postgres
--

CREATE TRIGGER trg_articulos_fecha_actualizacion BEFORE UPDATE ON public.articulos FOR EACH ROW EXECUTE FUNCTION public.actualizar_fecha_modificacion();

--
-- TOC entry 4752 (class 2606 OID 67973)
-- Name: articulos articulos_categoriaid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.articulos
ADD CONSTRAINT articulos_categoriaid_fkey FOREIGN KEY (categoriaid) REFERENCES public.categorias (id);

--
-- TOC entry 4753 (class 2606 OID 67978)
-- Name: articulos articulos_proveedorid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.articulos
ADD CONSTRAINT articulos_proveedorid_fkey FOREIGN KEY (proveedorid) REFERENCES public.proveedores (id);

--
-- TOC entry 4754 (class 2606 OID 67993)
-- Name: logoperaciones logoperaciones_usuarioid_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.logoperaciones
ADD CONSTRAINT logoperaciones_usuarioid_fkey FOREIGN KEY (usuarioid) REFERENCES public.usuarios (id);

-- Completed on 2025-10-25 16:19:19

--
-- PostgreSQL database dump complete
--
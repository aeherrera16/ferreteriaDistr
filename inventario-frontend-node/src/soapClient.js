// --- Categorías ---

export async function obtenerCategorias(token) {
  const client = await soap.createClientAsync(WSDL);
  const [result] = await client.ObtenerTodasCategoriasAsync({ token });
  const datos = result?.ObtenerTodasCategoriasResult?.Datos;
  let lista = [];
  if (datos) {
    if (Array.isArray(datos.Categoria)) {
      lista = datos.Categoria;
    } else if (datos.Categoria) {
      lista = [datos.Categoria];
    }
  }
  return lista;
}

export async function crearCategoria(data) {
  // TODO: Llamar al método SOAP real para crear categoría
  // Simulación: no hacer nada
  return true;
}

// --- Proveedores ---

export async function obtenerProveedores(token) {
  const client = await soap.createClientAsync(WSDL);
  const [result] = await client.ObtenerTodosProveedoresAsync({ token });
  const datos = result?.ObtenerTodosProveedoresResult?.Datos;
  let lista = [];
  if (datos) {
    if (Array.isArray(datos.Proveedor)) {
      lista = datos.Proveedor;
    } else if (datos.Proveedor) {
      lista = [datos.Proveedor];
    }
  }
  return lista;
}

export async function crearProveedor(data) {
  // TODO: Llamar al método SOAP real para crear proveedor
  return true;
}
// Autenticación SOAP: devuelve el token si es exitoso
export async function autenticarSoap(nombreUsuario, password) {
  const client = await soap.createClientAsync(WSDL);
  try {
    const [result] = await client.AutenticarAsync({ nombreUsuario, password });
    console.log('Respuesta SOAP Autenticar:', JSON.stringify(result, null, 2));
    if (result?.AutenticarResult?.Exito && result.AutenticarResult.Datos) {
      return result.AutenticarResult.Datos;
    } else {
      // Mostrar mensaje de error detallado si existe
      throw new Error(result?.AutenticarResult?.Mensaje || 'Error de autenticación SOAP');
    }
  } catch (e) {
    // Mostrar error completo en consola
    console.error('Error autenticando contra SOAP:', e);
    throw e;
  }
}
import soap from 'soap';
import dotenv from 'dotenv';
dotenv.config();

const WSDL = process.env.SOAP_WSDL;


// Ahora recibe el token y lo envía junto al artículo

// El backend espera los campos como argumentos individuales
export async function insertarArticulo(dto, token) {
  const client = await soap.createClientAsync(WSDL);
  const {
    Codigo,
    Nombre,
    Descripcion,
    Categoria,
    PrecioCompra,
    PrecioVenta,
    Stock,
    StockMinimo,
    Proveedor
  } = dto;
  // El backend espera: token, codigo, nombre, descripcion, categoriaId, precioCompra, precioVenta, stock, stockMinimo, proveedorId
  const [result] = await client.InsertarArticuloAsync({
    token,
    codigo: Codigo,
    nombre: Nombre,
    descripcion: Descripcion,
    categoriaId: Categoria ? Number(Categoria) : null,
    precioCompra: PrecioCompra,
    precioVenta: PrecioVenta,
    stock: Stock,
    stockMinimo: StockMinimo,
    proveedorId: Proveedor ? Number(Proveedor) : null
  });
  return result?.InsertarArticuloResult;
}

export async function consultarPorCodigo(codigo, token) {
  const client = await soap.createClientAsync(WSDL);
  // El backend espera { token, codigo }
  const [result] = await client.ConsultarArticuloPorCodigoAsync({ token, codigo });
  return result?.ConsultarArticuloPorCodigoResult;
}

// Buscar artículo por nombre (filtrado en frontend si no hay método directo)
export async function consultarPorNombre(nombre, token) {
  const client = await soap.createClientAsync(WSDL);
  // Si el backend no tiene método directo, obtenemos todos y filtramos
  const [result] = await client.ObtenerTodosArticulosAsync({ token });
  const datos = result?.ObtenerTodosArticulosResult?.Datos;
  let lista = [];
  if (datos) {
    if (Array.isArray(datos.Articulo)) {
      lista = datos.Articulo;
    } else if (datos.Articulo) {
      lista = [datos.Articulo];
    }
  }
  if (!Array.isArray(lista) || lista.length === 0) {
    return null;
  }
  // Coincidencia exacta o parcial (case-insensitive)
  const nombreLower = nombre.trim().toLowerCase();
  // Buscar coincidencia exacta primero
  let encontrado = lista.find(a => a.Nombre && a.Nombre.trim().toLowerCase() === nombreLower);
  if (!encontrado) {
    // Si no hay coincidencia exacta, buscar parcial
    encontrado = lista.find(a => a.Nombre && a.Nombre.toLowerCase().includes(nombreLower));
  }
  return encontrado || null;
}

// Actualizar artículo por id
export async function actualizarArticulo(id, dto, token) {
  const client = await soap.createClientAsync(WSDL);
  // Construir objeto Articulo esperado por el servicio
  const articuloObj = {
    id: Number(id),
    codigo: dto.Codigo,
    nombre: dto.Nombre,
    descripcion: dto.Descripcion,
    categoriaId: dto.Categoria ? Number(dto.Categoria) : null,
    precioCompra: dto.PrecioCompra,
    precioVenta: dto.PrecioVenta,
    stock: dto.Stock,
    stockMinimo: dto.StockMinimo,
    proveedorId: dto.Proveedor ? Number(dto.Proveedor) : null
  };
  // Llamada SOAP: ActualizarArticuloAsync espera (token, id, articulo) según contrato
  const [result] = await client.ActualizarArticuloAsync({ token, id: Number(id), articulo: articuloObj });
  return result?.ActualizarArticuloResult;
}

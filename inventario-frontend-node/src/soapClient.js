import soap from 'soap';
import dotenv from 'dotenv';
dotenv.config();

const WSDL = process.env.SOAP_WSDL;

export async function insertarArticulo(dto) {
  const client = await soap.createClientAsync(WSDL);
  // Llamada al método según el WSDL
  const [result] = await client.InsertarArticuloAsync({ articulo: dto });
  return result?.InsertarArticuloResult;
}

export async function consultarPorCodigo(codigo) {
  const client = await soap.createClientAsync(WSDL);
  const [result] = await client.ConsultarArticuloPorCodigoAsync({ codigo });
  return result?.ConsultarArticuloPorCodigoResult;
}

import Joi from 'joi';

export const articuloSchema = Joi.object({
  Codigo: Joi.string().max(30).required(),
  Nombre: Joi.string().max(120).required(),
  Categoria: Joi.string().max(80).allow(''),
  PrecioCompra: Joi.number().positive().required(),
  PrecioVenta: Joi.number().positive().greater(Joi.ref('PrecioCompra')).required(),
  Stock: Joi.number().integer().min(0).required(),
  StockMinimo: Joi.number().integer().min(0).required(),
  Proveedor: Joi.string().max(120).allow('')
});

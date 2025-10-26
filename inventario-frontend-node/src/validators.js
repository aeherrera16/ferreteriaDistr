import Joi from 'joi';

export const articuloSchema = Joi.object({
  Codigo: Joi.string().max(30).required(),
  Nombre: Joi.string().max(120).required(),
  Descripcion: Joi.string().max(255).allow(''),
  Categoria: Joi.string().max(80).allow(''),
  PrecioCompra: Joi.number().min(0).required().messages({
    'number.min': 'No se permiten valores negativos en Precio compra.'
  }),
  PrecioVenta: Joi.number().min(0).greater(Joi.ref('PrecioCompra')).required().messages({
    'number.min': 'No se permiten valores negativos en Precio venta.'
  }),
  Stock: Joi.number().integer().min(0).required().messages({
    'number.min': 'No se permiten valores negativos en Stock.'
  }),
  StockMinimo: Joi.number().integer().min(0).required().messages({
    'number.min': 'No se permiten valores negativos en Stock Mínimo.'
  }),
  Proveedor: Joi.string().max(120).allow('')
});

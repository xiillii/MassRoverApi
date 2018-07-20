﻿using System;
using System.Collections.Generic;
using System.Linq;
using MassRoverApi.Errors;
using MassRoverApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace MassRoverApi.Controllers
{
    [Produces("application/json")]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private static List<Product> _products = new List<Product>
        {
            new Product {Id = 1, Name = "Lithium L2", ModifiedDate = DateTime.UtcNow.AddDays(-2)},
            new Product {Id = 2, Name = "SNU 61", ModifiedDate = DateTime.UtcNow.AddDays(-20)}
        };


        /// <summary>
        /// Get a list of all prodcts
        /// </summary>
        /// <returns>List of products</returns>
        /// <response code="200">List of products</response>
        [HttpGet]
        [ProducesResponseType(typeof(List<Product>), 200)]
        public IActionResult GetProducts()
        {
            return Ok(_products);
        }


        /// <summary>
        /// Gets product by id
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns>Product</returns>
        /// <response code="200">Product</response>
        /// <response code="404">No Product found for the specified id</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Product), 200)]
        [ProducesResponseType(typeof(EntityNotFoundErrorMessage), 404)]
        public IActionResult GetProductById(int id)
        {
            var product = _products.SingleOrDefault(p => p.Id == id);

            if (product != null)
            {
                return Ok(product);
            }
            else
            {
                return NotFound(ErrorService.GetEntityNotFoundErrorMessage(typeof(Product), id));
            }
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="product">New Product</param>
        /// <returns>Product</returns>
        /// <response code="201">Created Product for the request</response>
        [HttpPost]
        [ProducesResponseType(typeof(Product), 201)]
        public IActionResult CreateProduct([FromBody] Product product)
        {
            product.Id = _products.Count + 1;
            
            _products.Add(product);

            return CreatedAtRoute(new {id = product.Id}, product);
        }

        /// <summary>
        /// Replaces a product
        /// </summary>
        /// <param name="id">New version of the Product</param>
        /// <param name="product">New version of the product</param>
        /// <returns></returns>
        /// <response code="204">No Content</response>
        /// <response code="400">Request mistmatch</response>
        /// <response code="404">No Product found for the specified id</response>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(RequestContentErrorMessage), 400)]
        [ProducesResponseType(typeof(EntityNotFoundErrorMessage), 404)]
        public IActionResult UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest(ErrorService.GetRequestContentMismatchErrorMessage());
            }

            var existingProduct = _products.SingleOrDefault(p => p.Id == product.Id);

            if (existingProduct != null)
            {
                existingProduct = product;
                existingProduct.ModifiedDate = DateTime.UtcNow;
            }
            else
            {
                return NotFound(ErrorService.GetEntityNotFoundErrorMessage(typeof(Product), product.Id));
            }

            return NoContent();
        }

        /// <summary>
        /// Deletes a product
        /// </summary>
        /// <param name="id">Product id</param>
        /// <returns></returns>
        /// <response code="204">No Content</response>
        /// <response code="404">No product found for the specified id</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(EntityNotFoundErrorMessage), 404)]
        public IActionResult DeleteProduct(int id)
        {
            var product = _products.SingleOrDefault(p => p.Id == id);

            if (product != null)
            {
                _products.Remove(product);
            }
            else
            {
                return NotFound(ErrorService.GetEntityNotFoundErrorMessage(typeof(Product), id));
            }

            return NoContent();
        }
    }
}
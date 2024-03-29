﻿using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using WakeCommerceCRUDProduct.Application.DTOs;
using WakeCommerceCRUDProduct.Application.Interfaces.Services;
using WakeCommerceCRUDProduct.Domain.Entities;

namespace WakeCommerceCRUDProduct.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productService = productService;


        /// <summary>
        /// Obter todos os Produtos
        /// </summary>
        /// <returns>Coleção de Produtos</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="500">Erro interno</response>
        [HttpGet("GetAllProducts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                var products = await _productService.GetAllProductAsync();

                return Ok(products);

            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }


        /// <summary>
        /// Obter um Produto por Id
        /// </summary>
        /// <param name="id">Identificador do Produto</param>
        /// <returns>Dados do Produto especificado</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("GetById/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByIdAsync(int id)
        {

            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product != null)
                return Ok(product);

                return NotFound("Produto Nao Encontrado");
            }
            catch
            {
                return NotFound("Produto Nao Encontrado");
            }
            
        }


        /// <summary>
        /// Obter um Produto pelo Nome
        /// </summary>
        /// <param name="name">Campo para trazer o Produto em especifico</param>
        /// <returns>Dados do Produto especificado</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="404">Não encontrado</response>
        [HttpGet("GetByName/{name}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByNameAsync(string name)
        {
            try
            {
                var product = await _productService.GetProductByNameAsync(name);

                if (product != null)
                    return Ok(product);

                return NotFound("Produto Nao Encontrado");
            }
            catch
            {
                return NotFound("Produto Nao Encontrado");
            }
        }
        /// <summary>
        /// Obter uma coleção de Produtos ordenados
        /// </summary>
        /// <param name="name">Escolha entre Name, Stock ou Value para trazer a ordenação</param>
        /// <returns>Lista de dados ordenada dos Produtos</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Bad Request</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("GetOrderByNameOrStockOrValue/{name}")]
        public async Task<IActionResult> OrderByProductAsync(string name)
        {
            try 
            {
                var products = await _productService.OrderByProductListAsync(name);
                if (products == null || !products.Any())
                    return BadRequest("Produtos Nao Encontrados");

                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }
        /// <summary>
        /// Cadastrar um Produto
        /// </summary>
        /// <remarks>
        /// {
        /// "name": "string",
        /// "stock": 0,
        /// "value": 00.00
        /// }
        /// </remarks>
        /// <param name="productDTO">Dados do Produto</param>
        /// <returns>Produto recém criado</returns>
        /// <response code="200">Sucesso</response>
        [HttpPost("Create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAsync(ProductDTOCreateAndUpdate productDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                string valorCorrigido = productDTO.Value.Replace(",", ".", StringComparison.InvariantCulture);
                //caso o usuario entre com , ou . a api aceitara e salvara no banco da mesma forma como Decimal
                if (!decimal.TryParse(valorCorrigido, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valorDecimal))
                {
                    return BadRequest("O valor fornecido não é um número válido.");
                }
                var product = new Product(productDTO.Name, productDTO.Stock, valorDecimal);

                var createdProduct = await _productService.CreateProductAsync(product);
                return Ok(createdProduct);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// Atualizar um Produto
        /// </summary>
        /// <remarks>
        /// {
        /// "name": "string",
        /// "stock": 0,
        /// "value": 00.00
        /// }
        /// </remarks>
        /// <param name="id">Identificador do Produto</param>
        /// <param name="productDTO">Dados do Produto que deseja atualizar</param>
        /// <returns>Sem Conteudo</returns>
        /// <response code="200">Sucesso</response>
        /// <response code="400">Bad Request</response>
        [HttpPut("Update/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAsync(int id, ProductDTOCreateAndUpdate productDTO)
        {

            try
            {
                string valorCorrigido = productDTO.Value.Replace(",", ".", StringComparison.InvariantCulture);
                //caso o usuario entre com , ou . a api aceitara e salvara no banco da mesma forma como Decimal
                if (!decimal.TryParse(valorCorrigido, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal valorDecimal))
                {
                    return BadRequest("O valor fornecido não é um número válido.");
                }

                var product = new Product(productDTO.Name, productDTO.Stock, valorDecimal);

                var existingProduct = await _productService.UpdateProductAsync(id, product);
                if (existingProduct == 0)
                    return BadRequest();

                return Ok("Produto atualizado com Sucesso");

            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deletar um Produto
        /// </summary>
        /// <param name="id">Identificador do Produto</param>
        /// <returns>Sem Conteudo</returns>
        /// <response code="204">Sucesso</response>
        /// <response code="400">Bad Request</response>
        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int product = await _productService.DeleteProductAsync(id);
                if (product == 0)
                    return BadRequest();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}

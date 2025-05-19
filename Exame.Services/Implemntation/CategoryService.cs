using Exame.Services.DataTransferObjects;
using Exame.Services.Interfaces;
using HirBot.Comman.Enums;
using HirBot.Common.Helpers;
using HirBot.Data.Entities;
using HirBot.ResponseHandler.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Project.Repository.Repository;
using Project.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Services.DataTransferObjects.Authencation;
using User.Services.models;

namespace Exame.Services.Implemntation
{
    public class CategoryService : ICategoryService
    { 
        private readonly IAuthenticationService _authenticationService;
        private readonly UnitOfWork _unitOfWork;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

        public CategoryService(IAuthenticationService authenticationService , UnitOfWork unitOfWork ) {
        
            _authenticationService = authenticationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<APIOperationResponse<object>> create(CategoryDto dto)
        {
            try
            { 
                var user=await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Company )
                    return APIOperationResponse<object>.UnOthrized("this email is not company");
               var category= new Category();
                category.Name = dto.Name;
                category.Description = dto.Description;
                category.UserID = user.Id;
                if (dto.image != null && (dto.image.Length > 0))
                {
                    try
                    {

                        string extension = Path.GetExtension(dto.image.FileName).ToLower();
                        if (!AllowedExtensions.Contains(extension))
                        {

                            return APIOperationResponse<object>.BadRequest("can not uplode the image", new
                            {
                                image = "can not uplode the image"
                            });
                        }
                        using var stream = dto.image.OpenReadStream();

                        string fileUrl = await FileHelper.UploadFileAsync(stream,  "categoryimage" + extension, "company");
                        category.image = fileUrl;
                    }
                    catch (Exception ex)
                    {
                        return APIOperationResponse<object>.BadRequest("can not uplode the file", new
                        {

                            image = "can not uplode the file"
                        });
                    }
                }
                _unitOfWork._context.Categories.Add(category);
               await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success(new { name = category.Name, description = category.Description, id = category.ID, createdAt = category.CreationDate, image = category.image }, "the category is created");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured"); 
            }
        }

        public async Task<APIOperationResponse<object>> GetALL(string? search = null)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Company)
                    return APIOperationResponse<object>.UnOthrized("this email is not company");
                
                var categories = _unitOfWork._context.Categories
                    .Select(c => new
                    {
                        c.ID,
                        c.Name,
                        c.Description,
                        c.image ,
                        c.UserID , 
                        c.CreationDate ,
                      c.exams
                       
                    }).Where(c=>c.UserID==user.Id)
                    .ToList();
                if (search != null)
                    categories = categories.Where(c => c.Name.StartsWith(search)).ToList();
                List<object> respone= new List<object>();
                foreach (var category in categories)
                {
                    respone.Add(new { numofexams= category.exams.Count(),  name = category.Name, description = category.Description, id = category.ID, createdAt = category.CreationDate, image = category.image });
                }
                 
                return APIOperationResponse<object>.Success(respone);
            }
            catch (Exception)
            {
                return APIOperationResponse<object>.ServerError("There was an error fetching the categories");
            }
        }

        public async Task<APIOperationResponse<object>> Delete(int id)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Company)
                    return APIOperationResponse<object>.UnOthrized("this email is not company");

                var category = _unitOfWork._context.Categories.FirstOrDefault(c => c.ID == id && c.UserID==user.Id);
                if (category == null)
                    return APIOperationResponse<object>.NotFound("This category is not found");

                _unitOfWork._context.Categories.Remove(category);
                await _unitOfWork.SaveAsync();

                return APIOperationResponse<object>.Success("Category deleted successfully");
            }
            catch (Exception)
            {
                return APIOperationResponse<object>.ServerError("An error occurred while deleting the category");
            }
        }

        public async Task<APIOperationResponse<object>> update(int id , CategoryDto dto)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Company)
                    return APIOperationResponse<object>.UnOthrized("this email is not company");
                var category = _unitOfWork._context.Categories.FirstOrDefault(c => c.ID == id && c.UserID == user.Id);

                if (category==null)
                    return APIOperationResponse<object>.NotFound("this category is not found");
                category.Name = dto.Name;
                category.Description = dto.Description;
                if(dto.removeImage==true)
                {
                    category.image = null;
                    
                }
                if (dto.image != null && (dto.image.Length > 0))
                {
                    try
                    {

                        string extension = Path.GetExtension(dto.image.FileName).ToLower();
                        if (!AllowedExtensions.Contains(extension))
                        {

                            return APIOperationResponse<object>.BadRequest("can not uplode the image", new
                            {
                                image = "can not uplode the image"
                            });
                        }
                        using var stream = dto.image.OpenReadStream();

                        string fileUrl = await FileHelper.UpdateFileAsync(stream, category.image,"categoryimage" + extension, "company");
                        category.image = fileUrl;
                    }
                    catch (Exception ex)
                    {
                        return APIOperationResponse<object>.BadRequest("can not uplode the file", new
                        {

                            image = "can not uplode the file"
                        });
                    }
                }
                _unitOfWork._context.Categories.Update(category);
                await _unitOfWork.SaveAsync();
                return APIOperationResponse<object>.Success(new { name = category.Name, description = category.Description, id = category.ID, createdAt = category.CreationDate, image = category.image }, "the category is updated");
            }
            catch (Exception ex)
            {
                return APIOperationResponse<object>.ServerError("there are error accured");
            }
        }
        public async Task<APIOperationResponse<object>> GetCategory(int id)
        {
            try
            {
                var user = await _authenticationService.GetCurrentUserAsync();
                if (user.role != UserType.Company)
                    return APIOperationResponse<object>.UnOthrized("this email is not company");

                var category = _unitOfWork._context.Categories
                    .Where(c => c.ID == id)
                    .Select(c => new
                    {
                        c.ID,
                        c.Name,
                        c.Description,
                        c.image , 
                        c.exams
                    })
                    .FirstOrDefault();

                if (category == null)
                    return APIOperationResponse<object>.NotFound("Category not found");

                return APIOperationResponse<object>.Success(new {category.Name , category.Description , category.image , category.ID , numofexams = category.exams.Count() });
            }
            catch (Exception)
            {
                return APIOperationResponse<object>.ServerError("There was an error fetching the category");
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using DatingApp.API.Controllers;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DatingApp.Tests
{
  public class PhotosControllerTests
  {
    private Mock<IDatingRepository> _datingRepository;
    private PhotosController _controller;
    private readonly ClaimsPrincipal _userClaims;

    public PhotosControllerTests()
    {
      _datingRepository = new Mock<IDatingRepository>();

      var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfiles()); });
      var mapper = mockMapper.CreateMapper();

      // Create fake app settings values for cloudinary provider
      var settings = new CloudinarySettings()
      {
        ApiKey = "A",
        ApiSecret = "B",
        CloudName = "C"
      };
      var someOptions = Options.Create(settings);

      _controller = new PhotosController(_datingRepository.Object, mapper, someOptions);

      // Mock user claims
      _userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
      {
        new Claim(ClaimTypes.Name, "palermo"),
        new Claim(ClaimTypes.NameIdentifier, "2"),
      }, "mock"));
      _controller.ControllerContext = new ControllerContext()
      {
        HttpContext = new DefaultHttpContext() { User = _userClaims }
      };
    }

    [Fact]
    public void GetPhoto_WhenPhotoExists_ReturnsRightPhoto()
    {
      var photo = GetFakePhotoList().SingleOrDefault(x => x.Id == 1);
      _datingRepository.Setup(repo => repo.GetPhoto(1)).ReturnsAsync(photo);

      var result = _controller.GetPhoto(1).Result;

      var okResult = Assert.IsType<OkObjectResult>(result);
      var returnValue = Assert.IsType<PhotoForReturnDto>(okResult.Value);
      Assert.Equal(photo.Description, returnValue.Description);
    }

    private ICollection<Photo> GetFakePhotoList()
    {
      return new List<Photo>()
      {
        new Photo()
        {
            Id = 1,
            IsMain = true,
            Description = "First"
        },
        new Photo()
        {
            Id = 2,
            IsMain = false,
            Description = "Second"
        }
      };
    }

  }
}
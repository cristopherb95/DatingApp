using System;
using Xunit;
using Moq;
using DatingApp.API.Data;
using DatingApp.API.Controllers;
using AutoMapper;
using DatingApp.API.Helpers;
using System.Collections.Generic;
using DatingApp.API.Models;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DatingApp.Tests
{
  public class UsersControllerTests
  {
    private Mock<IDatingRepository> _usersRepository;
    private UsersController _controller;

    private readonly ClaimsPrincipal _userClaims;

    public UsersControllerTests()
    {
      _usersRepository = new Mock<IDatingRepository>();

      var mockMapper = new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapperProfiles()); });
      var mapper = mockMapper.CreateMapper();

      _controller = new UsersController(_usersRepository.Object, mapper);

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
    public void GetUser_WhenCalled_ReturnsRightUser()
    {
      var user = GetFakeUserList().SingleOrDefault(x => x.Id == 2);
      _usersRepository.Setup(repo => repo.GetUser(2, It.IsAny<bool>())).ReturnsAsync(user);

      var result = _controller.GetUser(2).Result;

      var okResult = Assert.IsType<OkObjectResult>(result);
      var returnValue = Assert.IsType<UserForDetailedDto>(okResult.Value);
      Assert.Equal(user.UserName, returnValue.Username);
    }

    [Fact]
    public void GetUsers_WhenCalled_ReturnsListOfUsers()
    {
      var userParams = new UserParams();
      var user = GetFakeUserList().First(x => x.Id == 2);
      var users = new PagedList<User>(GetFakeUserList().ToList(), 1, 1, 1);

      _usersRepository.Setup(repo => repo.GetUser(2, It.IsAny<bool>())).ReturnsAsync(user);
      _usersRepository.Setup(repo => repo.GetUsers(userParams)).ReturnsAsync(users);

      var result = _controller.GetUsers(userParams).Result;

      var okResult = Assert.IsType<OkObjectResult>(result);
      var returnValue = Assert.IsType<List<UserForListDto>>(okResult.Value);
      Assert.Equal(users.Count, returnValue.Count());
    }

    [Fact]
    public void UpdateUser_UnauthorizedUserClaims_ReturnsUnauthorized()
    {
      var userUpdateData = new UserForUpdateDto
      {
        City = "Palermo",
        Country = "Italy",
        Interests = "Tetris",
        Introduction = "Hi"
      };

      var result = _controller.UpdateUser(1, userUpdateData).Result;

      var createdAtRouteResult = Assert.IsType<UnauthorizedResult>(result);
    }

    [Fact]
    public void UpdateUser_RightUserClaims_ReturnsNoContent()
    {
      var userUpdateData = new UserForUpdateDto
      {
        City = "Palermo",
        Country = "Italy",
        Interests = "Tetris",
        Introduction = "Hi"
      };
      // var user = GetFakeUserList().SingleOrDefault(x => x.Id == 2);
      // _usersRepository.Setup(repo => repo.GetUser(2)).ReturnsAsync(user);
      _usersRepository.Setup(repo => repo.SaveAll()).ReturnsAsync(true);

      var result = _controller.UpdateUser(2, userUpdateData).Result;

      var createdAtRouteResult = Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async void UpdateUser_SaveFails_ThrowsException()
    {
      int id = 2;
      var userUpdateData = new UserForUpdateDto
      {
        City = "Palermo",
        Country = "Italy",
        Interests = "Tetris",
        Introduction = "Hi"
      };
      _usersRepository.Setup(repo => repo.SaveAll()).ReturnsAsync(false);

      Exception ex = await Assert.ThrowsAsync<Exception>(() => _controller.UpdateUser(id, userUpdateData));

      Assert.Equal(ex.Message, $"Updating user {id} failed on save");
    }

    private ICollection<User> GetFakeUserList()
    {
      return new List<User>()
      {
        new User()
        {
            Id = 1,
            UserName = "Bob",
            Gender = "male"
        },
        new User()
        {
            Id = 2,
            UserName = "George",
            Gender = "male"
        }
      };
    }

  }
}

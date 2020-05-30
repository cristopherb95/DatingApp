using System;
using DatingApp.API.Data;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;
using System.Collections.Generic;

namespace DatingApp.Tests
{
  public class DatingRepositoryTests
  {
    private DbContextOptions<DataContext> _options;
    private DatingRepository _datingRepository;
    public DatingRepositoryTests()
    {
      string dbName = Guid.NewGuid().ToString();
      _options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(databaseName: dbName).Options;

      Seed();
    }

    [Fact]
    public void GetLike_WhenCalled_ReturnsRightLike()
    {
      var like = _datingRepository.GetLike(1, 2).Result;

      var result = Assert.IsType<Like>(like);
      Assert.Equal(1, result.LikerId);
    }

    [Fact]
    public void GetMessage_WhenCalled_ReturnsRightMessage()
    {
      var message = _datingRepository.GetMessage(2).Result;

      var result = Assert.IsType<Message>(message);
      Assert.Equal("Test2", result.Content);
    }

    [Fact]
    public void GetMessagesForUser_WhenCalled_ReturnsPageListWithMessage()
    {
      var messagesForUser = _datingRepository.GetMessagesForUser(new MessageParams { UserId = 1 }).Result;

      var result = Assert.IsType<PagedList<Message>>(messagesForUser);
      Assert.Equal(1, result.Count);
    }

    [Fact]
    public void GetMessageThread_WhenCalled_ReturnsMessagesBetweenUsers()
    {
      var messageThread = _datingRepository.GetMessageThread(1, 2).Result;

      var result = Assert.IsType<List<Message>>(messageThread);
      Assert.Equal(2, result.Count());
    }

    [Fact]
    public void GetMainPhotoForUser_WhenCalled_ReturnsMainPhoto()
    {
      var mainPhoto = _datingRepository.GetMainPhotoForUser(1).Result;

      var result = Assert.IsType<Photo>(mainPhoto);
      Assert.Equal("TestPhoto3", result.Description);
    }

    private void Seed()
    {
      var dbContext = new DataContext(_options);
      dbContext.Database.EnsureDeleted();
      dbContext.Database.EnsureCreated();

      var photos = new Photo[]
      {
        new Photo { Id = 1, UserId = 1, Description = "TestPhoto1", IsApproved = true, IsMain = false },
        new Photo { Id = 2, UserId = 1, Description = "TestPhoto2", IsApproved = true, IsMain = false },
        new Photo { Id = 3, UserId = 1, Description = "TestPhoto3", IsApproved = true, IsMain = true }
      };

      var users = new User[]
      {
          new User { Id = 1, UserName = "Bob", KnownAs = "Bobby", Photos = photos},
          new User { Id = 2, UserName = "George", KnownAs = "Georgie"}
      };

      var likes = new Like[]
      {
          new Like { Liker = users[0], Likee = users[1], LikerId = 1, LikeeId = 2}
      };

      var messages = new Message[]
      {
        new Message { Id = 1, Content = "Test1", Sender = users[0], Recipient = users[1], SenderId = 1, RecipientId = 2},
        new Message { Id = 2, Content = "Test2", Sender = users[1], Recipient = users[0], SenderId = 2, RecipientId = 1}
      };

      dbContext.Likes.AddRange(likes);
      dbContext.Messages.AddRange(messages);
      dbContext.Photos.AddRange(photos);
      dbContext.Users.AddRange(users);

      dbContext.SaveChanges();

      _datingRepository = new DatingRepository(dbContext);
    }

  }
}
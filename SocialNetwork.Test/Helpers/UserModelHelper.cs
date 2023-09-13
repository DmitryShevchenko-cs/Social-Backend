﻿using System.Text;
using SocialNetwork.BL.Models;
using SocialNetwork.BL.Models.Enums;

namespace SocialNetwork.Test.Helpers;

public static class UserModelHelper
{
    public static async Task<UserModel> CreateTestData()
    {
        Random random = new Random();
        return new UserModel()
        {

            Login = GenerateRandomLogin(8, 60),
            Password = "Password",
            Profile = new ProfileModel()
            {
                Birthday = DateTime.Now,
                Description = "sdsdds",
                Email = "User" + random.Next(100, 10000) + "@gmail.com",
                Name = "Test",
                Sex = Sex.Male,
                Surname = "Test",
                AvatarImage = "Image"
            }
        };
    }
    
    private static readonly Random random = new();

    public static string GenerateRandomLogin(int minLength, int maxLength)
    {
        int loginLength = random.Next(minLength, maxLength + 1);
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        StringBuilder loginBuilder = new StringBuilder();

        for (int i = 0; i < loginLength; i++)
        {
            int randomIndex = random.Next(chars.Length);
            loginBuilder.Append(chars[randomIndex]);
        }

        return loginBuilder.ToString();
    }
    
}
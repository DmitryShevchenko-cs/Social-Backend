﻿namespace SocialNetwork.BL.Exceptions;

public class UserNotFoundException : CustomException
{
    public UserNotFoundException(string message) : base(message)
    {
    }
}
﻿namespace SocialNetwork.BL.Exceptions;

public class ChatMemberException : CustomException
{
    public ChatMemberException(string message) : base(message)
    {
    }
}
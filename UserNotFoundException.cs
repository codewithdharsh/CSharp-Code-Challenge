﻿using System;

public class UserNotFoundException : Exception
{
    public UserNotFoundException(string message) : base(message) { }
}

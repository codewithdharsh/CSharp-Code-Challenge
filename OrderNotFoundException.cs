﻿using System;

public class OrderNotFoundException : Exception
{
    public OrderNotFoundException(string message) : base(message) { }
}

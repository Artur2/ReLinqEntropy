﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace ReLinqEntropy.Query.Transformation
{
    public interface IMethodCallTransformerProvider
    {
        IMethodCallTransformer GetTransformer(MethodCallExpression methodCallExpression);
    }
}

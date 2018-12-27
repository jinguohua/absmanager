using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ChineseAbs.ABSManagement.Utils
{
    internal enum ExpressionOperator
    {
        None,
        Add,
        Subtract
    }

    internal class ExpressionNode
    {
        internal ExpressionNode(ExpressionTree tree)
        {
            m_tree = tree;
        }

        private ExpressionTree m_tree;
        private ExpressionNode Left;
        private ExpressionNode Right;
        private ExpressionOperator Operator;
        private string Text;

        private ExpressionOperator ParseOperator(char op)
        {
            if (op == '+')
            {
                return ExpressionOperator.Add;
            }
            else if (op == '-')
            {
                return ExpressionOperator.Subtract;
            }

            CommUtils.Assert(false, "ExpressionNode parse operator [" + op + "] failed.");
            return ExpressionOperator.None;
        }

        internal void Parse(string expression)
        {
            Text = expression;

            if (expression.Any(m_ops.Contains))
            {
                var op = expression.First(x => m_ops.Contains(x));
                var index = expression.LastIndexOf(op);
                var l = expression.Substring(0, index).Trim();
                var r = expression.Substring(index + 1, expression.Length - index - 1).Trim();
                Left = new ExpressionNode(m_tree);
                Left.Parse(l);

                Right = new ExpressionNode(m_tree);
                Right.Parse(r);

                Operator = ParseOperator(op);
            }
            else
            {
                Operator = ExpressionOperator.None;
            }
        }

        internal Expression ParseValue(string expression)
        {
            bool isPercent = expression.EndsWith("%");
            if (isPercent)
            {
                expression = expression.Substring(0, expression.Length - 1);
            }

            double d;
            if (double.TryParse(expression, out d))
            {
                if (isPercent)
                {
                    d /= 100.0d;
                }
                return Expression.Constant(d, typeof(double));
            }

            if (m_tree.Params != null && m_tree.Params.ContainsKey(expression))
            {
                object param = m_tree.Params[expression];
                return Expression.Constant(Convert.ToDouble(param), typeof(double));
            }

            return null;
        }

        internal Expression ConvertExpression()
        {
            if (Operator == ExpressionOperator.None)
            {
                return ParseValue(Text);
            }

            var l = Left.ConvertExpression();
            var r = Right.ConvertExpression();
            switch (Operator)
            {
                case ExpressionOperator.Add:
                    return Expression.Add(l, r);
                case ExpressionOperator.Subtract:
                    return Expression.Subtract(l, r);
            }

            return null;
        }

        private readonly char[] m_ops = new char[] { '+', '-' };
    }

    internal class ExpressionTree
    {
        public Dictionary<string, object> Params { get; set; }

        internal void Load(string expression)
        {
            m_node = new ExpressionNode(this);
            m_node.Parse(expression);
        }

        internal T Eval<T>()
        {
            var val = m_node.ConvertExpression();
            Expression<Func<T>> lambda = Expression.Lambda<Func<T>>(val);
            return lambda.Compile()();
        }

        private ExpressionNode m_node;
    }

    /// <summary>
    /// 表达式求值
    /// e.g.
    ///     new ExpressionUtils(1+1).Eval<int>() = 2
    /// </summary>
    public class ExpressionUtils
    {
        public ExpressionUtils(string expression)
        {
            m_tree = new ExpressionTree();
            m_expression = expression;
        }

        public ExpressionUtils AddParam(string key, object instance)
        {
            if (m_tree.Params == null)
            {
                m_tree.Params = new Dictionary<string, object>();
            }

            m_tree.Params.Add(key, instance);
            return this;
        }

        public T Eval<T>()
        {
            m_tree.Load(m_expression);
            return m_tree.Eval<T>();
        }

        private string m_expression;
        private ExpressionTree m_tree;
    }
}

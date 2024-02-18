namespace MinAssignment
{
    interface Exp
    {
        int eval(Env env);
    }

    class Env
    {
        public Dictionary<string, int> env;
        public Dictionary<string, Func> funcs;
        public Env parent = null;
        public Env()
        {
            env = new Dictionary<string, int>();
            funcs = new Dictionary<string, Func>();
        }
        public Env(ref Env env)
        {
            this.parent = env;
            this.env = new Dictionary<string, int>();
            this.funcs = new Dictionary<string, Func>();
        }
        public void set(string var, int val)
        {
            if(env.ContainsKey(var))
            {
                throw new Exception("already defined variable: " + var);
            }
            else
            {
                env.Add(var, val);
            }
        }
        public void assign(string var, int val)
        {
            if (env.ContainsKey(var))
            {
                env[var] = val;
            }
            else if (parent != null && parent.ContainsVar(var))
            {
                parent.assign(var, val);
            }
            else
            {
                throw new Exception("undefined variable: " + var);
            }
        }
        
        public int get(string var)
        {
            if(env.ContainsKey(var))
            {
                return env[var];
            }
            else if (parent != null && parent.ContainsVar(var))
            {
                return parent.get(var);
            }
            else
            {
                throw new Exception("undefined variable: " + var);
            }
        }
        public bool ContainsVar(string var)
        {
              return env.ContainsKey(var) || (parent != null && parent.ContainsVar(var));
        }

        public void setFunc(string func, Func f)
        {
            if (funcs.ContainsKey(func))
            {
                funcs[func] = f;
            }
            else if (parent != null && parent.ContainsFunc(func))
            {
                parent.setFunc(func, f);
            }
            else
            {
                funcs.Add(func, f);
            }
        }
        public Func getFunc(string func)
        {
            if (funcs.ContainsKey(func))
            {
                return funcs[func];
            }
            else if (parent != null && parent.ContainsFunc(func))
            {
                return parent.getFunc(func);
            }
            else
            {
                throw new Exception("undefined function: " + func);
            }
        }

        public bool ContainsFunc(string func)
        {
            return funcs.ContainsKey(func) || (parent != null && parent.ContainsFunc(func));
        }
    }

    class Num : Exp
    {
        private int n;
        public Num(int n)
        {
            this.n = n;
        }
        public int eval(Env env)
        {
            return n;
        }
    }

    #region Culc
    class Add : Exp
    {
        private Exp e1, e2;
        public Add(Exp e1, Exp e2)
        {
            this.e1 = e1;
            this.e2 = e2;
        }
        public int eval( Env env)
        {
            return e1.eval(env) + e2.eval(env);
        }
    }

    class Mul : Exp
    {
        private Exp e1, e2;
        public Mul(Exp e1, Exp e2)
        {
            this.e1 = e1;
            this.e2 = e2;
        }
        public int eval(Env env)
        {
            return e1.eval(env) * e2.eval(env);
        }
    }
    #endregion

    #region condition
    class Eq : Exp
    {
        private Exp e1, e2;
        public Eq(Exp e1, Exp e2)
        {
            this.e1 = e1;
            this.e2 = e2;
        }
        public int eval(Env env)
        {
            return e1.eval(env) == e2.eval(env) ? 1 : 0;
        }
    }

    class Lt : Exp
    {
        private Exp e1, e2;
        public Lt(Exp e1, Exp e2)
        {
            this.e1 = e1;
            this.e2 = e2;
        }
        public int eval(Env env)
        {
            return e1.eval(env) < e2.eval(env) ? 1 : 0;
        }
    }

    class Gt : Exp
    {
        private Exp e1, e2;
        public Gt(Exp e1, Exp e2)
        {
            this.e1 = e1;
            this.e2 = e2;
        }
        public int eval(Env env)
        {
            return e1.eval(env) > e2.eval(env) ? 1 : 0;
        }
    }
    #endregion

    #region Seq
    class Sequence : Exp
    {
        private List<Exp> exps;
        public Sequence(List<Exp> exps)
        {
            this.exps = exps;
        }

        public int eval(Env env)
        {
            var localEnv = new Env(ref env);
            int result = 0;
            foreach (Exp e in exps)
            {
                result = e.eval(localEnv);
            }
            return result;
        }
    }

    class If : Exp
    {
        private Exp cond, then, els;
        public If(Exp cond, Exp then, Exp els)
        {
            this.cond = cond;
            this.then = then;
            this.els = els;
        }
        public int eval(Env env)
        {
            if (cond.eval(env) == 1)
            {
                return then.eval(env);
            }
            else
            {
                return els.eval(env);
            }
        }
    }

    class While : Exp
    {
        private Exp cond, body;
        public While(Exp cond, Exp body)
        {
            this.cond = cond;
            this.body = body;
        }
        public int eval(Env env)
        {
            int result = 0;
            while (cond.eval(env) == 1)
            {
                result = body.eval(env);
            }
            return result;
        }
    }
    #endregion

    #region variable
    class Let : Exp
    {
        private string var;
        private Exp exp;
        public Let(string var, Exp exp)
        {
            this.var = var;
            this.exp = exp;
        }
        public int eval(Env env)
        {
            int val = exp.eval(env);
            env.set(var, val);
            return val;
        }
    }

    class  Assign : Exp
    {
        private string var;
        private Exp exp;
        public Assign(string var, Exp exp)
        {
            this.var = var;
            this.exp = exp;
        }
        public int eval(Env env)
        {
            int val = exp.eval(env);
            env.assign(var, val);
            return val;
        }
    }

    class Eval : Exp
    {
        private string var;
        public Eval(string var)
        {
            this.var = var;
        }
        public int eval(Env env)
        {
            return env.get(var);
        }
    }
    #endregion

    class PrintNum : Exp
    {
        private Exp exp;
        public PrintNum(Exp exp)
        {
            this.exp = exp;
        }
        public int eval(Env env)
        {
            int val = exp.eval(env);
            Console.WriteLine(val);
            return val;
        }
    }

    class None : Exp
    {
        public int eval(Env env)
        {
            return 0;
        }
    }

    interface Func : Exp
    {
        int call(List<Exp> exps, Env env);
    }

    class FuncDef : Func
    {
        string name;
        private List<string> args;
        private Exp body;
        public FuncDef(string name, List<string> args, Exp body)
        {
            this.args = args;
            this.body = body;
            this.name = name;
        }
        public int eval(Env env)
        {
            env.setFunc(name, this);
            return 0;
        }

        public int call(List<Exp> exps, Env env)
        {
            for (int i = 0; i < args.Count; i++)
            {
                env.set(args[i], exps[i].eval(env));
            }
            return body.eval(env);
        }
    }

    class FuncCall : Exp
    {
        private string func;
        private List<Exp> exps;
        public FuncCall(string func, List<Exp> exps)
        {
            this.func = func;
            this.exps = exps;
        }
        public int eval(Env env)
        {
            var localEnv = new Env(ref env);   
            return env.getFunc(func).call(exps, localEnv);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            Env env = new Env();
            var program = new Sequence(new List<Exp>
            {
                new FuncDef(
                    "fib", 
                    new List<string>{ "i"},
                    new Sequence(new List<Exp>{
                        //new PrintNum(new Eval("i")),
                        new If(
                            new Lt(new Eval("i"), new Num(3)),
                            new Num(1),
                            new Add(
                                new FuncCall("fib", new List<Exp>{ new Add(new Eval("i"), new Num(-1)) }),
                                new FuncCall("fib", new List<Exp>{ new Add(new Eval("i"), new Num(-2)) })
                            )
                        )
                    })
                ),
                new Let("i", new Num(10)),
                new While(new Gt (new Eval ("i"), new Num(0)),
                    new Sequence(new List<Exp>
                    {
                        new PrintNum(new FuncCall("fib", new List<Exp>{ new Eval("i")})),
                        new Assign("i", new Add(new Eval("i"), new Num(-1))),
                    })),
            });
            Console.WriteLine(program.eval(env));

        }
    }
}
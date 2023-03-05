using System;
using System.Collections.Generic;

using UnityEngine;


namespace DiGro.ScreenSystem {

    public class Context {
        public int id = 0;
    }

    public class ScreenContext : Context { }
    public class DialogContext : Context { }

    public class DialogResult {
        public int id = 0;
    }

}

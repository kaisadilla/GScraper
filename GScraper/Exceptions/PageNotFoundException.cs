using System;
using System.Collections.Generic;
using System.Text;

namespace Kaisa.GScraper.Exceptions {
    public class PageNotFoundException : Exception {

        public PageNotFoundException() { }

        public PageNotFoundException(string message) : base(message) { }

        public PageNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleAppWiki
{
    public class MainItem
    {
        public string a { get; set; }
    }

    public class Warnings
    {
        public MainItem main { get; set; }
    }

    public class Item//22936
    {
        public int pageid { get; set; }
        public int ns { get; set; }
        public string title { get; set; }
        public string extract { get; set; }
    }

    public class Pages
    {
        public Item item { get; set; }
    }

    public class Query
    {
        public Pages pages { get; set; }
    }

    public class Example
    {
        public Warnings warnings { get; set; }
        public string batchcomplete { get; set; }
        public Query query { get; set; }
    }
}


﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Transactions;

namespace Sciendo.Topper.Notifier
{
    public static class Style
    {
        public static string Definition => @"<style>
table, td, th {    
    border: 1px solid #ddd;
    text-align: left;
}

table {
    border-collapse: collapse;
    width: 100%;
font-family:sans-serif;

}

th, td {
    padding: 5px;

}
th {
background-color:#f2dcea;
}
.today {
font-size:1.25em;
}
.first{
background-color:#ed7c71;
}
.second{
background-color:#f2f28c;
}
.third{
background-color:#9df28c;
}
.current{
font-weight:bold;
font-size:1.25em;
}
</style>";

        public static string Today => "today";

        public static string First => "first";

        public static string Second => "second";

        public static string Third => "third";

        public static string Current => "current";
    }
}


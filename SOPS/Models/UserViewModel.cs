﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SOPS.Models
{
    public class UserProfileViewModel
    {
        public string Id                            { get; set; }
        public string UserName                      { get; set; }
        public string Name                          { get; set; }
        public string Surname                       { get; set; }
        public string PhoneNumber                   { get; set; }
        public string Email                         { get; set; }
        public bool IsEmployee                      { get; set; }
        public List<WatchedProduct> WatchedProducts { get; set; }
        public Company Company                      { get; set; }
    }
}
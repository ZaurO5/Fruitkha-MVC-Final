﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.ViewModels.News;

public class NewsDetailsVM
{
	public string Title { get; set; }
	public string Body { get; set; }
	public string Photo { get; set; }
    public DateTime CreatedAt { get; set; }
}

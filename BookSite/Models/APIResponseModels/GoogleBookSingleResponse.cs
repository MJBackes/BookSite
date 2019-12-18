using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookSite.Models.APIResponseModels
{

    public class GoogleBookSingleResponse
    {
        public string kind { get; set; }
        public string id { get; set; }
        public string etag { get; set; }
        public string selfLink { get; set; }
        public Volumeinfo volumeInfo { get; set; }
        public SingleSaleinfo saleInfo { get; set; }
        public Accessinfo accessInfo { get; set; }
    }

    public class SingleVolumeinfo
    {
        public string title { get; set; }
        public string subtitle { get; set; }
        public string[] authors { get; set; }
        public string publisher { get; set; }
        public string publishedDate { get; set; }
        public string description { get; set; }
        public SingleIndustryidentifier[] industryIdentifiers { get; set; }
        public int pageCount { get; set; }
        public Dimensions dimensions { get; set; }
        public string printType { get; set; }
        public string mainCategory { get; set; }
        public string[] categories { get; set; }
        public double averageRating { get; set; }
        public int ratingsCount { get; set; }
        public string contentVersion { get; set; }
        public SingleImagelinks imageLinks { get; set; }
        public string language { get; set; }
        public string previewLink { get; set; }
        public string infoLink { get; set; }
        public string canonicalVolumeLink { get; set; }
    }

    public class Dimensions
    {
        public string height { get; set; }
        public string width { get; set; }
        public string thickness { get; set; }
    }

    public class SingleImagelinks
    {
        public string smallThumbnail { get; set; }
        public string thumbnail { get; set; }
        public string small { get; set; }
        public string medium { get; set; }
        public string large { get; set; }
        public string extraLarge { get; set; }
    }

    public class SingleIndustryidentifier
    {
        public string type { get; set; }
        public string identifier { get; set; }
    }

    public class SingleSaleinfo
    {
        public string country { get; set; }
        public string saleability { get; set; }
        public bool isEbook { get; set; }
        public Listprice listPrice { get; set; }
        public Retailprice retailPrice { get; set; }
        public string buyLink { get; set; }
    }

    public class Listprice
    {
        public float amount { get; set; }
        public string currencyCode { get; set; }
    }

    public class Retailprice
    {
        public float amount { get; set; }
        public string currencyCode { get; set; }
    }

    public class SingleAccessinfo
    {
        public string country { get; set; }
        public string viewability { get; set; }
        public bool embeddable { get; set; }
        public bool publicDomain { get; set; }
        public string textToSpeechPermission { get; set; }
        public SingleEpub epub { get; set; }
        public SinglePdf pdf { get; set; }
        public string accessViewStatus { get; set; }
    }

    public class SingleEpub
    {
        public bool isAvailable { get; set; }
        public string acsTokenLink { get; set; }
    }

    public class SinglePdf
    {
        public bool isAvailable { get; set; }
    }

}
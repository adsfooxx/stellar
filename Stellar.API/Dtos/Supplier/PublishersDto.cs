namespace Stellar.API.Dtos.Supplier
{
    public class PublishersDto
    {
        public int PublisherId { get; set; }
        public string PublisherName {  get; set; }
        //聯絡人(負責人)
        public string ConcatName {  get; set; }
        public string Country {  get; set; }
        public string Address {  get; set; }
        public string Phone { get; set; }
        public string Status {  get; set; }
    }
}

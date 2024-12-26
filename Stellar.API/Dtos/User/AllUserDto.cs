namespace Stellar.API.Dtos.User
{
    public class AllUserDto
    {
        public int UserId { get; set; }
        public string NickName { get; set; }
        public string Account { get; set; }
        public decimal Wallet { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public byte IsLock { get; set; }


    }
    // 用户新增 DTO
    public class AddUserDto
    {
        public string Account { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public decimal Wallet { get; set; }
        public byte IsLock { get; set; }
    }

    // 用户更新 DTO
    public class UpdateUserDto
    {
        public int UserId { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public decimal Wallet { get; set; }
        public byte IsLock { get; set; }
    }


}

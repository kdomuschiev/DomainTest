namespace DomainTest
{
    internal class StaffMember
    {
        public StaffMember()
        {
        }

        public string UserID { get; internal set; }
        public string DisplayName { get; internal set; }
        public string GivenName { get; internal set; }
        public string Surname { get; internal set; }
    }
}
namespace Api.Auth{

    /// <summary>
    /// There are the hard-coded roles available in the system
    /// </summary>
    public class AuthRoles
    {
        /// <summary>
        /// Super user system.
        /// Overall access to information about all working groups and users
        /// </summary>
        public const string ROLE_SUPERUSER = "SUPERUSER";

        /// <summary>
        /// University admin.
        /// Access to all features within university group
        /// </summary>
        public const string ROLE_UNIVERSITY_ADMIN = "UNIVERSITYADMIN";

        /// <summary>
        /// University user.
        /// Access to information which university admin decide
        /// </summary>
        public const string ROLE_USNIVERSITY_USER = "UNIVERSITYUSER";
    }
}
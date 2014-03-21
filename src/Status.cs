using System;

namespace TinyWebStack
{
    public class Status
    {
        public Status(int code, string description = null, string location = null)
        {
            this.Code = code;
            this.Description = description;
            this.Location = location;
        }

        public int Code { get; private set; }

        public string Description { get; private set; }

        public string Location { get; private set; }

        public bool Success
        {
            get { return this.Code >= 200 && this.Code <= 299; }
        }

        public static readonly Status OK = new Status(200, "OK");

        public static readonly Status Created = new Status(201, "Created");

        public static Status CreatedAt(string location)
        {
            return new Status(201, "Created", location);
        }

        public static readonly Status Accepted = new Status(202, "Accepted");

        public static Status MovedPermanentlyTo(string location)
        {
            return new Status(301, "Moved Permanently", location);
        }

        public static Status FoundAt(string location)
        {
            return new Status(302, "Found", location);
        }

        public static Status SeeOtherHere(string location)
        {
            return new Status(303, "See Other", location);
        }

        public static readonly Status NotModified = new Status(304, "Not Modified");

        public static Status TemporaryRedirectTo(string location)
        {
            return new Status(307, "Temporary Redirect", location);
        }

        public static readonly Status BadRequest = new Status(400, "Bad Request");

        public static readonly Status Unauthorized = new Status(401, "Unauthorized");

        public static readonly Status Forbidden = new Status(403, "Forbidden");

        public static readonly Status NotFound = new Status(404, "Not Found");

        public static readonly Status MethodNotAllowed = new Status(405, "MethodNotAllowed");

        public static readonly Status Conflict = new Status(409, "Conflict");

        public static readonly Status Gone = new Status(410, "Gone");

        public static readonly Status UnprocessableEntity = new Status(422, "Unprocessable Entity");

        public static readonly Status InternalServerError = new Status(500, "Internal Server Error");

        public static readonly Status NotImplemented = new Status(501, "Not Implemented");

        public override bool Equals(object obj)
        {
            Status other = obj as Status;

            return (other != null && this.Code == other.Code);
        }

        public override int GetHashCode()
        {
            return this.Code;
        }

        public override string ToString()
        {
            return String.IsNullOrEmpty(this.Description) ? this.Code.ToString() : String.Concat(this.Code, " ", this.Description);
        }
    }
}

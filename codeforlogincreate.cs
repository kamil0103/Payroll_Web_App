using System;
//FOR CREATING USERS
using Payroll.Hashing;

// For registration:
 var stored = PasswordHasher.HashPassword(plainTextPassword);
// Save `stored` in your Users table 


//FOR LOGGING IN USERS
// Retrieve the user's stored hash string from DB
var stored = user.PasswordHash;

bool ok = PasswordHasher.Verify(enteredPassword, stored);
if (!ok)
{
    // invalid credentials
}
else
{
    // login success
    // Optionally upgrade the hash if parameters changed:
    if (PasswordHasher.NeedsRehash(stored))
    {
        user.PasswordHash = PasswordHasher.HashPassword(enteredPassword);
        // save changes to Database
    }
}


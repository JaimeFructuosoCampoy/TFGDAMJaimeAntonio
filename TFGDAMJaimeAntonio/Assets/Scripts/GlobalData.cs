using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalData : MonoBehaviour
{
    public static bool GameOver = false;
    public static bool PlayerLoggedIn = false;
    public const string SUPABASE_DB_URL = "https://bxjubueuyzobmpvdwefk.supabase.co/rest/v1/";
    public const string SUPABASE_DB_KEY = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImJ4anVidWV1eXpvYm1wdmR3ZWZrIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDQyMzc5MDYsImV4cCI6MjA1OTgxMzkwNn0.C0GxUJFC8mj622Aq8vnLo6Qw9jI_6RHdN70n6T87e9Q";
}

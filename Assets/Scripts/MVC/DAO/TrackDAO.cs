using MVC.Global;
using MVC.Models;
using MVC.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVC.DAO
{
    public class TrackDAO : Connection
    {
        public TrackDAO() : base() { }

        public List<Track> ListAll()
        {
            try
            {
                return Factory<Track>.CreateMany(ExecuteQuery(Configuration.Queries.Track.ListAll));
            }
            catch (Exception ex)
            {
                Debug.LogErrorFormat("ScoreboardDAO::ListAll -> {0}", ex.Message);
                throw ex;
            }
            finally
            {
                CloseConnection();
            }
        }
    }
}
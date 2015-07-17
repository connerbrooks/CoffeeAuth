﻿using CoffeeAuth.Models;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeAuth
{
    class DrinkerDatabase
    {
        private static DrinkerDatabase database;
        public SQLiteConnection conn;

        public static DrinkerDatabase Instance 
        {
            get
            {
                if (database == null)
                    database = new DrinkerDatabase();
                return database;
            }
        }

        public DrinkerDatabase()
        {
            conn = new SQLiteConnection("coffeepeople.db");
            string s = @"CREATE TABLE IF NOT EXISTS
                            Customer (Id    INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                                Name        VARCHAR( 140 ),
                                BadgeCIN    VARCHAR( 140 ),
                                PictureUrl  VARCHAR( 140 ),
                                Balance     INTEGER,
                                NumBags     INTEGER,
                                NumMilks    INTEGER,
                                NumShots    INTEGER,
                                NumLogins   INTEGER
                            );";

            using (var statement = conn.Prepare(s))
            {
                statement.Step();
            }
        }

        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            using (var statement = conn.Prepare("SELECT Name, Balance, BadgeCIN FROM Customer"))
            {
                while (SQLiteResult.ROW == statement.Step())
                {
                    var user = new User()
                    {
                        Name = (string)statement[0],
                        Balance = (long)statement[1],
                        BadgeCIN = (string)statement[2]
                    };
                    users.Add(user);
                }
            }
            return users;
        }


        public void createUser(String name, String badgeCIN)
        {

            try
            {
                using (var userstmt = conn.Prepare("INSERT INTO Customer (Name, BadgeCIN, BALANCE) VALUES (?, ?, ?)"))
                {
                    userstmt.Bind(1, name);
                    userstmt.Bind(2, badgeCIN);
                    userstmt.Bind(3, 1); // initial balance of 1
                    userstmt.Step();
                }
            }
            catch
            {
                // handle error
            }
        }

        public User GetUser(string badgeCIN)
        {
            User user = null;

            using (var statement = conn.Prepare("SELECT BadgeCIN, Name, Balance FROM Customer WHERE BadgeCIN = ?"))
            {

                statement.Bind(1, badgeCIN);
                if (SQLiteResult.ROW == statement.Step())
                {
                        user = new User()
                        {
                            BadgeCIN = (string)statement[0],
                            Name = (string)statement[1],
                            Balance = (long)statement[2]
                        };
                }
            }
            return user;
        }

        public void UpdateUser(User user)
        {
            var existingUser = Instance.GetUser(user.BadgeCIN);
            if (existingUser != null)
            {
                using (var custstmt = conn.Prepare("UPDATE Customer SET Balance = ?, Name = ?, PictureUrl = ?, NumBags = ?, NumMilks = ?, NumShots = ?, NumLogins = ? WHERE BadgeCIN=?"))
                {
                    custstmt.Bind(1, user.Balance);
                    custstmt.Bind(2, user.Name);
                    custstmt.Bind(3, user.PictureUrl);
                    custstmt.Bind(4, user.NumBags);
                    custstmt.Bind(5, user.NumMilks);
                    custstmt.Bind(6, user.NumShots);
                    custstmt.Bind(7, user.NumLogins);
                    custstmt.Bind(8, user.BadgeCIN);
                    custstmt.Step();
                }
            }
        }

    }
}
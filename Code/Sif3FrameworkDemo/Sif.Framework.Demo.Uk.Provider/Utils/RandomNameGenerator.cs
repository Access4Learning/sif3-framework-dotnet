/*
 * Copyright 2015 Systemic Pty Ltd
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace Sif.Framework.Demo.Uk.Provider.Utils
{

    /// <summary>
    /// This class is used to generate random names.
    /// </summary>
    public class RandomNameGenerator
    {
        private static Random random = new Random();
        private static string[] familyNames = { "Smith", "Johnson", "Williams", "Jones", "Brown", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson", "Thomas", "Jackson", "White", "Harris", "Martin", "Thompson", "Garcia", "Martinez", "Robinson", "Clark", "Rodriguez", "Lewis", "Lee", "Walker", "Hall", "Allen", "Young", "Hernandez", "King", "Wright", "Lopez", "Hill", "Scott", "Green", "Adams", "Baker", "Gonzalez", "Nelson", "Carter", "Mitchell", "Perez", "Roberts", "Turner", "Phillips", "Campbell", "Parker", "Evans", "Edwards", "Collins", "Stewart", "Sanchez", "Morris", "Rogers", "Reed", "Cook", "Morgan", "Bell", "Murphy", "Bailey", "Rivera", "Cooper", "Richardson", "Cox", "Howard", "Ward", "Torres", "Peterson", "Gray", "Ramirez", "James", "Watson", "Brooks", "Kelly", "Sanders", "Price", "Bennett", "Wood", "Barnes", "Ross", "Henderson", "Coleman", "Jenkins", "Perry", "Powell", "Long", "Patterson", "Hughes", "Flores", "Washington", "Butler", "Simmons", "Foster", "Gonzales", "Bryant", "Alexander", "Russell", "Griffin", "Diaz", "Hayes" };
        private static string[] givenNames = { "Aiden", "Jackson", "Mason", "Liam", "Jacob", "Jayden", "Ethan", "Noah", "Lucas", "Logan", "Caleb", "Caden", "Jack", "Ryan", "Connor", "Michael", "Elijah", "Brayden", "Benjamin", "Nicholas", "Alexander", "William", "Matthew", "James", "Landon", "Nathan", "Dylan", "Evan", "Luke", "Andrew", "Gabriel", "Gavin", "Joshua", "Owen", "Daniel", "Carter", "Tyler", "Cameron", "Christian", "Wyatt", "Henry", "Eli", "Joseph", "Max", "Isaac", "Samuel", "Anthony", "Grayson", "Zachary", "David", "Christopher", "John", "Isaiah", "Levi", "Jonathan", "Oliver", "Chase", "Cooper", "Tristan", "Colton", "Austin", "Colin", "Charlie", "Dominic", "Parker", "Hunter", "Thomas", "Alex", "Ian", "Jordan", "Cole", "Julian", "Aaron", "Carson", "Miles", "Blake", "Brody", "Adam", "Sebastian", "Adrian", "Nolan", "Sean", "Riley", "Bentley", "Xavier", "Hayden", "Jeremiah", "Jason", "Jake", "Asher", "Micah", "Jace", "Brandon", "Josiah", "Hudson", "Nathaniel", "Bryson", "Ryder", "Justin", "Bryce", "Sophia", "Emma", "Isabella", "Olivia", "Ava", "Lily", "Chloe", "Madison", "Emily", "Abigail", "Addison", "Mia", "Madelyn", "Ella", "Hailey", "Kaylee", "Avery", "Kaitlyn", "Riley", "Aubrey", "Brooklyn", "Peyton", "Layla", "Hannah", "Charlotte", "Bella", "Natalie", "Sarah", "Grace", "Amelia", "Kylie", "Arianna", "Anna", "Elizabeth", "Sophie", "Claire", "Lila", "Aaliyah", "Gabriella", "Elise", "Lillian", "Samantha", "Makayla", "Audrey", "Alyssa", "Ellie", "Alexis", "Isabelle", "Savannah", "Evelyn", "Leah", "Keira", "Allison", "Maya", "Lucy", "Sydney", "Taylor", "Molly", "Lauren", "Harper", "Scarlett", "Brianna", "Victoria", "Liliana", "Aria", "Kayla", "Annabelle", "Gianna", "Kennedy", "Stella", "Reagan", "Julia", "Bailey", "Alexandra", "Jordyn", "Nora", "Carolin", "Mackenzie", "Jasmine", "Jocelyn", "Kendall", "Morgan", "Nevaeh", "Maria", "Eva", "Juliana", "Abby", "Alexa", "Summer", "Brooke", "Penelope", "Violet", "Kate", "Hadley", "Ashlyn", "Sadie", "Paige", "Katherine", "Sienna", "Piper" };

        /// <summary>
        /// Random family name.
        /// </summary>
        public static string FamilyName
        {

            get
            {
                return familyNames[random.Next(familyNames.Length)];
            }

        }

        /// <summary>
        /// Random given name.
        /// </summary>
        public static string GivenName
        {

            get
            {
                return givenNames[random.Next(givenNames.Length)];
            }

        }

    }

}

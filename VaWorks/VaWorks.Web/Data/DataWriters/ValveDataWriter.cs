using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VaWorks.Web.Data.Entities;

namespace VaWorks.Web.Data.DataWriters
{
    public class ValveDataWriter : IDataWriter
    {
        private ApplicationDbContext db;
        private int organizationId;


        public ValveDataWriter(ApplicationDbContext db, int organizationId)
        {
            this.db = db;
            this.organizationId = organizationId;
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }

        public void Write(IDataReader reader)
        {
            var organization = db.Organizations.Find(organizationId);

            while (reader.Read()) {
                // columns
                // mfg, model, size, ValveIntCode

                // get the valve interface
                int valveIntCode = reader.GetInt32(3);
                string mfg = reader.GetString(0);
                string model = reader.GetString(1);
                string size = reader.GetString(2);

                var valveInterface = db.ValveInterfaceCodes.Find(valveIntCode);
                if (valveInterface == null) {
                    valveInterface = new ValveInterfaceCode() {
                        InterfaceCode = valveIntCode
                    };
                    db.ValveInterfaceCodes.Add(valveInterface);
                }

                var valve = db.Valves
                    .Where(v => v.Manufacturer == mfg)
                    .Where(v => v.Model == model)
                    .Where(v => v.Size == size).FirstOrDefault();

                if(valve == null) {
                    valve = new Valve() {
                        Manufacturer = reader.GetString(0),
                        Model = reader.GetString(1),
                        Size = reader.GetString(2),
                        InterfaceCode = valveIntCode
                    };
                    db.Valves.Add(valve);
                }

                // link this valve to the organization
                if (valve.ValveId > 0) {
                    if (!organization.Valves.Where(v => v.ValveId == valve.ValveId).Any()) {
                        organization.Valves.Add(valve);
                    }
                } else {
                    organization.Valves.Add(valve);
                }
            }
        }
    }
}
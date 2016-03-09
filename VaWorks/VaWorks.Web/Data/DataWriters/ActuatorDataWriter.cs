using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VaWorks.Web.Data.Entities;

namespace VaWorks.Web.Data.DataWriters
{
    public class ActuatorDataWriter : IDataWriter
    {
        private ApplicationDbContext db;
        private int organizationId;


        public ActuatorDataWriter(ApplicationDbContext db, int organizationId)
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
                // mfg, model, size, ActuatorIntCode

                // get the actuator interface
                int actuatorIntCode = reader.GetInt32(3);
                string mfg = reader.GetString(0);
                string model = reader.GetString(1);
                string size = reader.GetString(2);

                var actuatorInterface = db.ActuatorInterfaceCodes.Find(actuatorIntCode);
                if (actuatorInterface == null) {
                    actuatorInterface = new ActuatorInterfaceCode() {
                        InterfaceCode = actuatorIntCode
                    };
                    db.ActuatorInterfaceCodes.Add(actuatorInterface);
                }

                var actuator = db.Actuators
                    .Where(v => v.Manufacturer == mfg)
                    .Where(v => v.Model == model)
                    .Where(v => v.Size == size).FirstOrDefault();

                if(actuator == null) {
                    actuator = new Actuator() {
                        Manufacturer = reader.GetString(0),
                        Model = reader.GetString(1),
                        Size = reader.GetString(2),
                        InterfaceCode = actuatorIntCode
                    };
                    db.Actuators.Add(actuator);
                }

                // link this actuator to the organization
                if (actuator.ActuatorId > 0) {
                    if (!organization.Actuators.Where(v => v.ActuatorId == actuator.ActuatorId).Any()) {
                        organization.Actuators.Add(actuator);
                    }
                } else {
                    organization.Actuators.Add(actuator);
                }
            }
        }
    }
}
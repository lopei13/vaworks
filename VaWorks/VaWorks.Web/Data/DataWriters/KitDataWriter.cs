using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VaWorks.Web.Data.Entities;

namespace VaWorks.Web.Data.DataWriters
{
    public class KitDataWriter : IDataWriter
    {
        private ApplicationDbContext db;
        private int organizationId;

        public KitDataWriter(ApplicationDbContext db, int organizationId)
        {
            this.db = db;
            this.organizationId = organizationId;
        }

        public void Write(System.Data.IDataReader reader)
        {
            var organization = db.Organizations.Find(organizationId);

            Dictionary<string, KitMaterial> materials = new Dictionary<string, KitMaterial>();
            Dictionary<string, KitOption> options = new Dictionary<string, KitOption>();
            Dictionary<int, ValveInterfaceCode> valveInterfaces = new Dictionary<int, ValveInterfaceCode>();
            Dictionary<int, ActuatorInterfaceCode> actuatorInterfaces = new Dictionary<int, ActuatorInterfaceCode>();

            while (reader.Read()) {
                // columns
                // KitNumber, ValveIntCode, ActIntCode, MaterialCode, OptionCode, Price
                string kitNumber = reader.GetString(0);
                var kit = db.Kits.Where(k => k.KitNumber == kitNumber).FirstOrDefault();

                if (kit == null) {
                    // get the material
                    string materialCode = reader.GetString(3);
                    var material = db.KitMaterials.Where(m => m.Code == materialCode).FirstOrDefault();

                    if (material == null) {
                        if (materials.ContainsKey(materialCode)) {
                            material = materials[materialCode];
                        } else {
                            material = new KitMaterial() {
                                Code = materialCode,
                                Name = materialCode
                            };
                            db.KitMaterials.Add(material);
                            materials.Add(materialCode, material);
                        }
                    }

                    // get the locking option
                    string optionCode = reader.GetString(4);
                    var option = db.KitOptions.Where(o => o.Code == optionCode).FirstOrDefault();

                    if (option == null) {
                        if (options.ContainsKey(optionCode)) {
                            option = options[optionCode];
                        } else {
                            option = new KitOption() {
                                Code = optionCode,
                                Name = optionCode
                            };
                            db.KitOptions.Add(option);
                            options.Add(optionCode, option);
                        }
                    }

                    // get the valve interface
                    int valveIntCode = reader.GetInt32(1);
                    var valveInterface = db.ValveInterfaceCodes.Find(valveIntCode);

                    if(valveInterface == null) {
                        if (valveInterfaces.ContainsKey(valveIntCode)) {
                            valveInterface = valveInterfaces[valveIntCode];
                        } else {
                            valveInterface = new ValveInterfaceCode() {
                                InterfaceCode = valveIntCode
                            };
                            db.ValveInterfaceCodes.Add(valveInterface);
                            valveInterfaces.Add(valveIntCode, valveInterface);
                        }
                    }

                    // get the actuator interace
                    int actIntCode = reader.GetInt32(2);
                    var actInterface = db.ActuatorInterfaceCodes.Find(actIntCode);

                    if (actInterface == null) {
                        if (actuatorInterfaces.ContainsKey(actIntCode)) {
                            actInterface = actuatorInterfaces[actIntCode];
                        } else {
                            actInterface = new ActuatorInterfaceCode() {
                                InterfaceCode = actIntCode
                            };
                            db.ActuatorInterfaceCodes.Add(actInterface);
                            actuatorInterfaces.Add(actIntCode, actInterface);
                        }
                    }

                    // add the kit
                    double price = reader.GetDouble(5);

                    kit = new Kit() {
                        KitNumber = reader.GetString(0),
                        ValuveIntefaceCodeEntity = valveInterface,
                        ActuatorInterfaceCodeEntity = actInterface,
                        Price = price,
                        Material = material,
                        Option = option
                    };

                    db.Kits.Add(kit);
                }

                // link this kit to the organization
                if (kit.KitId > 0) {
                    if (!organization.Kits.Where(k => k.KitId == kit.KitId).Any()) {
                        organization.Kits.Add(kit);
                    }
                } else {
                    organization.Kits.Add(kit);
                }
            }
        }

        public int SaveChanges()
        {
            return db.SaveChanges();
        }
    }
}
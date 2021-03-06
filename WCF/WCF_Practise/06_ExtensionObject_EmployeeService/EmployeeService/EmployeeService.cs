﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace EmployeeService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmployeeService" in both code and config file together.

    [ServiceBehavior(InstanceContextMode =InstanceContextMode.Single)]
    public class EmployeeService : IEmployeeService
    {
        private Employee _lastSavedEmployee;
        public void SaveEmployee(Employee employee)
        {
            string cs = ConfigurationManager.ConnectionStrings["subhasishConnectionString"].ConnectionString;
            _lastSavedEmployee = employee;
            using (SqlConnection con=new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spSaveEmployee",con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter parameterId = new SqlParameter()
                {
                    ParameterName = "@Id",
                    Value = employee.Id
                };
                cmd.Parameters.Add(parameterId);

                SqlParameter parameterName = new SqlParameter()
                {
                    ParameterName = "@Name",
                    Value = employee.Name
                };
                cmd.Parameters.Add(parameterName);

                //SqlParameter parameterGender = new SqlParameter()
                //{
                //    ParameterName = "@Gender",
                //    Value = employee.Gender
                //};
                //cmd.Parameters.Add(parameterGender);

                SqlParameter parameterDateOfBirth = new SqlParameter()
                {
                    ParameterName = "@DateOfBirth",
                    Value = employee.DateOfBirth
                };
                cmd.Parameters.Add(parameterDateOfBirth);

                SqlParameter parameterEmployeeType = new SqlParameter()
                {
                    ParameterName = "@EmployeeType",
                    Value = employee.Type
                };
                cmd.Parameters.Add(parameterEmployeeType);

                if (employee.GetType()==typeof(FullTimeEmployee))
                {
                    SqlParameter parameterAnnualSalary = new SqlParameter()
                    {
                        ParameterName = "@AnnualSalary",
                        Value = ((FullTimeEmployee)employee).AnnualSalary
                    };
                    cmd.Parameters.Add(parameterAnnualSalary);
                }
                else
                {
                    SqlParameter parameterHourlyPay = new SqlParameter()
                    {
                        ParameterName = "@HourlyPay",
                        Value = ((PartTimeEmployee)employee).HourlyPay
                    };
                    cmd.Parameters.Add(parameterHourlyPay);

                    SqlParameter parameterHoursWorked = new SqlParameter()
                    {
                        ParameterName = "@HoursWorked",
                        Value = ((PartTimeEmployee)employee).HoursWorked
                    };
                    cmd.Parameters.Add(parameterHoursWorked);

                    SqlParameter parameterCity = new SqlParameter()
                    {
                        ParameterName = "@City",
                        Value = employee.City
                    };
                    cmd.Parameters.Add(parameterCity);
                }
                con.Open();
                cmd.ExecuteNonQuery();




            }
        }

        public Employee GetEmployee(int id)
        {
            Employee emp = default(Employee);
            string cs = ConfigurationManager.ConnectionStrings["subhasishConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(cs))
            {
                SqlCommand cmd = new SqlCommand("spGetEmployee", con);
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter parameterId = new SqlParameter();
                parameterId.ParameterName = "@Id";
                parameterId.Value = id;

                cmd.Parameters.Add(parameterId);
                con.Open();

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    if ((EmployeeType)reader["EmployeeType"]==EmployeeType.FullTimeEmployee)
                    {
                        emp = new FullTimeEmployee()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            //Gender = reader["Gender"].ToString(),
                            DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                            Type = EmployeeType.FullTimeEmployee,
                            AnnualSalary = Convert.ToInt32(reader["AnnualSalary"]),
                            City = reader["City"].ToString(),
                        };
                    }
                    else
                    {
                        emp = new PartTimeEmployee()
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"].ToString(),
                            //Gender = reader["Gender"].ToString(),
                            DateOfBirth = Convert.ToDateTime(reader["DateOfBirth"]),
                            Type = EmployeeType.PartTimeEmployee,
                            HourlyPay = Convert.ToInt32(reader["HourlyPay"]),
                            HoursWorked = Convert.ToInt32(reader["HoursWorked"]),
                            City = reader["City"].ToString(),
                        };
                    }
                    
                }
            }

            if (_lastSavedEmployee!=null && _lastSavedEmployee.Id==id)
            {
                emp.ExtensionData = _lastSavedEmployee.ExtensionData;
            }
            return emp;
        }
    }
}

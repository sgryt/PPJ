using Automation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace AutomationTest
{
    public class ConvertToClaimRulesCommandTest
    {
        public const string noPermissionAssignmentsFile = ".\\PPJ rettigheder - ingen tildelinger.xlsx";
        public const string allPermissionsAssignmentsFile = ".\\PPJ rettigheder - alle tildelinger.xlsx";
        public const string duplicateRoleNamesFile = ".\\PPJ rettigheder - duplikerede rollenavne.xlsx";
        public const string duplicatePermissionsFile = ".\\PPJ rettigheder - duplikerede rettigheder.xlsx";

        [Theory, AutoConvertData]
        public void InvokeCanUseValidExcelFile(
            ConvertToClaimRulesCommand sut
        )
        {
            Assert.DoesNotThrow(() => {
                foreach (var result in sut.Invoke()) { }
            }
            );
        }

        [Theory, AutoConvertData]
        public void InvokeReturnsNoRecordsForEmptyAssignmentsFile(
            ConvertToClaimRulesCommand sut
        )
        {
            sut.RoleAssignmentFile = ConvertToClaimRulesCommandTest.noPermissionAssignmentsFile;

            Assert.Equal(0, sut.Invoke().OfType<RolePermissionClaimRule>().Count());
        }

        [Theory, AutoConvertData]
        public void InvokeReturnsCorrectNumberOfRecordsForAllAssignmentsFile(
            ConvertToClaimRulesCommand sut
        )
        {
            sut.RoleAssignmentFile = ConvertToClaimRulesCommandTest.allPermissionsAssignmentsFile;

            Assert.Equal(315, sut.Invoke().OfType<RolePermissionClaimRule>().Count());
        }

        [Theory, AutoConvertData]
        public void InvokeThrowsWhenDuplicateRoleNamesExist(
            ConvertToClaimRulesCommand sut
            )
        {
            sut.RoleAssignmentFile = ConvertToClaimRulesCommandTest.duplicateRoleNamesFile;

            Assert.Throws<InvalidDataException>(() =>
                sut.Invoke<RolePermissionClaimRule>().ToList()
            );
        }

        [Theory, AutoConvertData]
        public void InvokeThrowsWhenDuplicatePermissionsExist(
            ConvertToClaimRulesCommand sut
            )
        {
            sut.RoleAssignmentFile = ConvertToClaimRulesCommandTest.duplicatePermissionsFile;

            Assert.Throws<InvalidDataException>(() =>
                sut.Invoke<RolePermissionClaimRule>().ToList()
            );
        }

        [Theory, AutoConvertData]
        public void InvokeReturnsRecords(
            ConvertToClaimRulesCommand sut
        )
        {
            Assert.NotEqual(0, sut.Invoke().OfType<RolePermissionClaimRule>().Count());
        }

        [Theory, AutoConvertData]
        public void InvokeReturnsCorrectNumberOfRecords(
            ConvertToClaimRulesCommand sut
        )
        {
            Assert.Equal(60, sut.Invoke().OfType<RolePermissionClaimRule>().Count());
        }

        [Theory]
        // First row
        [InlineAutoConverteData("urn:dsdn:ppj:LoggePaaMobilEnhed", "LoggePaaMobilEnhed", "Logge på mobil enhed", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:LoggePaaMobilEnhed", "LoggePaaMobilEnhed", "Logge på mobil enhed", "...")]
        [InlineAutoConverteData("urn:dsdn:ppj:LoggePaaMobilEnhed", "LoggePaaMobilEnhed", "Logge på mobil enhed", "Rolle 127")]
        // include samples where not all roles get all permissions
        [InlineAutoConverteData("urn:dsdn:ppj:SeEgneTidligereJournaler", "SeEgneTidligereJournaler", "Se egne tidligere journaler ", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:SeEgneTidligereJournaler", "SeEgneTidligereJournaler", "Se egne tidligere journaler ", "...")]
        [InlineAutoConverteData("urn:dsdn:ppj:SoegeEfterTidligereJournalerForAktuellePatient", "SoegeEfterTidligereJournalerForAktuellePatient", "Søge efter tidligere journaler for aktuelle patient", "...")]
        [InlineAutoConverteData("urn:dsdn:ppj:SoegeEfterEgneTidligereJournaler", "SoegeEfterEgneTidligereJournaler ", "Søge efter egne tidligere journaler ", "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:OpretteBehandlingsplads", "OpretteBehandlingsplads", "Oprette behandlingsplads", "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:VisningAfBehandlingspladsTilknyttetAktuelHaendelse", "VisningAfBehandlingspladsTilknyttetAktuelHaendelse", "Visning af behandlingsplads tilknyttet aktuel hændelse", "...")]
        // ...snip..., include a sample for the case where all roles get the same permission
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereBhandlerpladsForEgenRegion", "RedigereBhandlerpladsForEgenRegion", "Redigere behandlerplads for egen region", "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereBhandlerpladsForEgenRegion", "RedigereBhandlerpladsForEgenRegion", "Redigere behandlerplads for egen region", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereBhandlerpladsForEgenRegion", "RedigereBhandlerpladsForEgenRegion", "Redigere behandlerplads for egen region", "...")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereBhandlerpladsForEgenRegion", "RedigereBhandlerpladsForEgenRegion", "Redigere behandlerplads for egen region", "Rolle 127")]
            // ...snip, check for last row in sheet
        [InlineAutoConverteData("urn:dsdn:ppj:BrugerAdministration", "BrugerAdministration", "Bruger administration", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:BrugerAdministration", "BrugerAdministration", "Bruger administration", "Rolle 127")]
        public void InvokeReturnsCorrectRecords(
            string permissionUri,
            string permissionValue,
            string title,
            string role,
            ConvertToClaimRulesCommand sut
            )
        {
            var actual = sut.Invoke<RolePermissionClaimRule>()
                .Where(cr =>
                    cr.Permission.AbsoluteUri == permissionUri
                    && cr.Value == permissionValue
                    && cr.Title == title
                    && cr.Role == role
                )
                .Count();

            Assert.Equal(1, actual);
        }

        [Theory]
        // First row
        [InlineAutoConverteData("urn:dsdn:ppj:LoggePaaMobilEnhed", "LoggePaaMobilEnhed", "Rolle 1")]
        // include samples where not all roles get all permissions
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereAktiveJournalerTilKnyttetAktuelleHaendelse", "RedigereAktiveJournalerTilKnyttetAktuelleHaendelse", "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereAktiveJournalerTilKnyttetAktuelleHaendelse", "RedigereAktiveJournalerTilKnyttetAktuelleHaendelse", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereAktiveJournalerTilKnyttetAktuelleHaendelse", "RedigereAktiveJournalerTilKnyttetAktuelleHaendelse", "...")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereAktiveJournalerTilKnyttetAktuelleHaendelse", "RedigereAktiveJournalerTilKnyttetAktuelleHaendelse", "Rolle 127")]
        [InlineAutoConverteData("urn:dsdn:ppj:SeEgneTidligereJournaler", "SeEgneTidligereJournaler",  "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:SeEgneTidligereJournaler", "SeEgneTidligereJournaler","Rolle 127")]
        [InlineAutoConverteData("urn:dsdn:ppj:SeEgneTidligereJournaler", "SeEgneTidligereJournaler", "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:SeEgneTidligereJournaler", "SeEgneTidligereJournaler", "Rolle 127")]
        [InlineAutoConverteData("urn:dsdn:ppj:SoegeEfterTidligereJournalerForAktuellePatient", "SoegeEfterTidligereJournalerForAktuellePatient", "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:SoegeEfterTidligereJournalerForAktuellePatient", "SoegeEfterTidligereJournalerForAktuellePatient", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:SoegeEfterTidligereJournalerForAktuellePatient", "SoegeEfterTidligereJournalerForAktuellePatient", "Rolle 127")]
        [InlineAutoConverteData("urn:dsdn:ppj:SoegeEfterEgneTidligereJournaler", "SoegeEfterEgneTidligereJournaler ", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:SoegeEfterEgneTidligereJournaler", "SoegeEfterEgneTidligereJournaler ", "...")]
        [InlineAutoConverteData("urn:dsdn:ppj:SoegeEfterEgneTidligereJournaler", "SoegeEfterEgneTidligereJournaler ", "Rolle 127")]
        [InlineAutoConverteData("urn:dsdn:ppj:OpretteBehandlingsplads", "OpretteBehandlingsplads", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:OpretteBehandlingsplads", "OpretteBehandlingsplads", "...")]
        [InlineAutoConverteData("urn:dsdn:ppj:OpretteBehandlingsplads", "OpretteBehandlingsplads", "Rolle 127")]
        [InlineAutoConverteData("urn:dsdn:ppj:VisningAfBehandlingspladsTilknyttetAktuelHaendelse", "VisningAfBehandlingspladsTilknyttetAktuelHaendelse", "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:VisningAfBehandlingspladsTilknyttetAktuelHaendelse", "VisningAfBehandlingspladsTilknyttetAktuelHaendelse", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:VisningAfBehandlingspladsTilknyttetAktuelHaendelse", "VisningAfBehandlingspladsTilknyttetAktuelHaendelse", "Rolle 127")]
        // Samples where no roles get permissions
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereEgneTidligereJournaler", "RedigereEgneTidligereJournaler", "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereEgneTidligereJournaler", "RedigereEgneTidligereJournaler", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereEgneTidligereJournaler", "RedigereEgneTidligereJournaler", "...")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereEgneTidligereJournaler", "RedigereEgneTidligereJournaler", "Rolle 127")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereTidligereJounaler", "RedigereTidligereJounaler", "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereTidligereJounaler", "RedigereTidligereJounaler", "Rolle 2")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereTidligereJounaler", "RedigereTidligereJounaler", "...")]
        [InlineAutoConverteData("urn:dsdn:ppj:RedigereTidligereJounaler", "RedigereTidligereJounaler", "Rolle 127")]
        // ...snip, check for last row in sheet
        [InlineAutoConverteData("urn:dsdn:ppj:BrugerAdministration", "BrugerAdministration", "Rolle 1")]
        [InlineAutoConverteData("urn:dsdn:ppj:BrugerAdministration", "BrugerAdministration", "...")]
        public void InvokeDoesNotReturnUnmarkedRecords(
                    string permissionUri,
                    string permissionValue,
                    string role,
                    ConvertToClaimRulesCommand sut
                    )
        {
            var actual = sut.Invoke<RolePermissionClaimRule>()
                .Where(cr =>
                    cr.Permission.AbsoluteUri == permissionUri
                    && cr.Value == permissionValue
                    && cr.Role == role
                )
                .Any();

            Assert.False(actual);
        }

    }
}

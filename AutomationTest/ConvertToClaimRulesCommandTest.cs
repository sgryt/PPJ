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

        public const string updatedLayoutFile = ".\\Bruger rettigheder 07022014.xlsx";
        public const string updatedLayoutFileWithRolesForAllPermissions = ".\\Bruger rettigheder 07022014 - alle permissions aktive.xlsx";
        public const string updatedLayoutFileWithRolesForAllPermissionsIncompleteShortnames = ".\\Bruger rettigheder 07022014 - alle permissions aktive - ukomplette shortnames.xlsx";
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
            sut.PermissionsUriPrefix = "http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/";

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
        [InlineAutoConverteData("urn:dsdn:ppj:SoegeEfterEgneTidligereJournaler", "SoegeEfterEgneTidligereJournaler", "Søge efter egne tidligere journaler ", "Rolle 1")]
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
            var claimRules = sut.Invoke<RolePermissionClaimRule>().ToList();
            var actual = claimRules
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
        
        [Theory, AutoConvertData]
        public void InvokeReturnsCorrectNumberOfRecordsForFileWithUpdatedLayout(
            ConvertToClaimRulesCommand sut
        )
        {
            sut.RoleAssignmentFile = ConvertToClaimRulesCommandTest.updatedLayoutFile;
            sut.PermissionsUriPrefix = "http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/";
            sut.PermissionIdsStartCell = new GridCoordinate(5, 0);
            sut.PermissionsShortNameColumnIndex = 1;
            sut.PermissionsTitleColumnIndex = 2;
            sut.RoleValuesStartCell = new GridCoordinate(2, 10);

            Assert.Equal(18, sut.Invoke().OfType<RolePermissionClaimRule>().Count());
        }

        [Theory]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "LoggePaaMobilEnhed", "Logge på mobil enhed")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "SoegeEfterTidligereJournalerForAktuellePatient", "Adgang til aktuelle patients tidligere PPJ Journaler")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "AdgangtilFMKinformationforaktuellepatient", "Adgang til FMK information for aktuelle patient")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Adgangtile-Journalforaktuellepatient", "Adgang til e-Journal for aktuelle patient")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Soegeefterogsejornalersomharvaerettilknyttetaktuelleberedskabindenforseneste24timer", "Søge efter og se jornaler som har været tilknyttet aktuelle beredskab inden for seneste 24 timer")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Soegeefterogsejournalersomaktuellebrugerharregistreretindenforsenesteuge", "Søge efter og se journaler som aktuelle bruger har registreret inden for seneste uge")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Soegeefterogsejournalerudfrapatientidentifikationindenforsenestemaaned", "Søge efter og se journaler udfra patient identifikation inden for seneste måned")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Registreijournalerpaaaktivhaendelse", "Registre i journaler på aktiv hændelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Redigeretidligerejournaler", "Redigere tidligere journaler")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "RegistreringafmedicinklasseA", "Registrering af medicinklasse A")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "RegistreringafmedicinklasseB", "Registrering af medicinklasse B")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "RegistreringafmedicinklasseC", "Registrering af medicinklasse C")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Oprettebehandlingsplads", "Oprette behandlingsplads")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Visningafbehandlingspladstilknyttetaktuelhaendelse", "Visning af behandlingsplads tilknyttet aktuel hændelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Oprettepatienterpaaenbehandlingsplads", "Oprette patienter på en behandlingsplads")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Oprettejournalerpaaenhaendelseviamobilenhed", "Oprette journaler på en hændelse via mobil enhed")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "LoggepaaCentralPPJ", "Logge på Central PPJ")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Oprettebeskedertilmobileenhedertilhoerendeegenregion", "Oprette beskeder til mobileenheder tilhørende egen region")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Seaktivejournaler-Nationalt", "Se aktive journaler - Nationalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Seaktivejournaler-Regionalt", "Se aktive journaler - Regionalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Seaktivejournaler-Egenmodtagelse", "Se aktive journaler - Egen modtagelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Seaktivejournalerhvorderikkeerregistreretmodtagelse-Nationalt", "Se aktive journaler hvor der ikke er registreret modtagelse - Nationalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Seaktivejournalerhvorderikkeerregistreretmodtagelse-Regionalt", "Se aktive journaler hvor der ikke er registreret modtagelse - Regionalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Soegeefterogseafsluttedejournalerudfrapatientinformation(CPR/Navn)-Nationalt", "Søge efter og se afsluttede journaler udfra patient information (CPR/Navn)  - Nationalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Soegeefterogseafsluttedejournalerudfrapatientinformation(CPR/Navn)-Regionalt", "Søge efter og se afsluttede journaler udfra patient information (CPR/Navn)  - Regionalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Soegeefterogseafsluttedejournalerudfrapatientinformation(CPR/Navn)-Egenmodtagelse", "Søge efter og se afsluttede journaler udfra patient information (CPR/Navn) - Egen modtagelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Soegeefterogseafsluttedejournalerudfraopgaveinformation(Optagested-Adresseetc.)-Nationalt", "Søge efter og se afsluttede journaler udfra opgave information (Optagested - Adresse etc.)  - Nationalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Soegeefterogseafsluttedejournalerudfraopgaveinformation(Optagested-Adresseetc.)-Regionalt", "Søge efter og se afsluttede journaler udfra opgave information (Optagested - Adresse etc.) - Regionalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Soegeefterogseafsluttedejournalerudfraopgaveinformation(Optagested-Adresseetc.)-Egenmodtagelse", "Søge efter og se afsluttede journaler udfra opgave information (Optagested - Adresse etc.) - Egen modtagelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Registrer'tilbagemeldingertiloperatoer'paajournalersommanharadgangtil", "Registrer 'tilbagemeldinger til operatør' på journaler som man har adgang til")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Sealle'tilbagemeldingertiloperatoer'paajournalersommanharadgangtil", "Se alle 'tilbagemeldinger til operatør' på journaler som man har adgang til")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Angivekapaciteterforegenmodtagelse", "Angive kapaciteter for egen modtagelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Angivekapaciteterformodtagelseriegenregion", "Angive kapaciteter for modtagelser i egen region")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Seogredigerebehandlerpladslogforegenregion", "Se og redigere behandlerpladslog for egen region")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Sebehandlerpladslogforegenregion", "Se behandlerpladslog for egen region")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Sebehandlerpladslogforandreregioner", "Se behandlerpladslog for andre regioner")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Adgangtilegenregionsstatistikwebinterface", "Adgang til egen regions statistik web interface")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Oprettenyestandardrapporteriegenregionsstatistikdatabase", "Oprette nye standard rapporter i egen regions statistikdatabase")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Adgangtilegenoperatoersstatistikwebinterface", "Adgang til egen operatørs statistik web interface")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Oprettenyestandardrapporteriegenoperatoersstatistikdatabase", "Oprette nye standard rapporter i egen operatørs statistikdatabase")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Redigerestamdata", "Redigere stamdata")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Selogfiler", "Se log filer")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Alarmopsaetning", "Alarmopsætning")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Seogsoegeefterjournaler-Egne", "Se og søge efter journaler - Egne")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Seogsoegeefterjournaler-Operatoer", "Se og søge efter journaler - Operatør")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Seogsoegeefterjournaler-Supervisorfor", "Se og søge efter journaler - Supervisor for")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Sekompetencebrug-Egen", "Se kompetencebrug - Egen")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Sekompetencebrug-Operatoer", "Se kompetencebrug - Operatør")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Sekompetencebrug-Supervisorfor", "Se kompetencebrug - Supervisor for")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Setilbagemeldingerpaajournalersombrugerenharadgangtil", "Se tilbagemeldinger på journaler som brugeren har adgang til")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Sekommentartiljournalersombrugerenharadgangtil", "Se kommentar til journaler som brugeren har adgang til")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Skriveogsekommentartiljournalersombrugerenharadgangtil", "Skrive og se kommentar til journaler som brugeren har adgang til")]
        public void PermissionTitlesAreReadCorrectly(
            string permissionUri,
            string permissionValue,
            string permissionTitle,
            ConvertToClaimRulesCommand sut
            )
        {
            sut.RoleAssignmentFile = ConvertToClaimRulesCommandTest.updatedLayoutFileWithRolesForAllPermissions;
            sut.PermissionsUriPrefix = "http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/";
            sut.PermissionIdsStartCell = new GridCoordinate(5, 0);
            sut.PermissionsShortNameColumnIndex = 1;
            sut.PermissionsTitleColumnIndex = 2;
            sut.RoleValuesStartCell = new GridCoordinate(2, 10);

            var claimRules = sut.Invoke<RolePermissionClaimRule>().ToList();
            var actual = sut.Permissions
                .Where(perm =>
                    perm.Uri.AbsoluteUri == permissionUri
                    && perm.ShortName == permissionValue
                )
                .Single()
                .Title;
            var claimRule = claimRules
                .Where(cr => cr.Permission.AbsoluteUri == permissionUri)
                .Where(cr => cr.Value == permissionValue)
                .Single();

            Assert.Equal(permissionTitle, actual);
            Assert.Equal(claimRule.Title, actual);
        }

        [Theory]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "LoggePaaMobilEnhed", "Logge på mobil enhed")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "AdgangTilAktuellePatientsTidligerePPJJournaler", "Adgang til aktuelle patients tidligere PPJ Journaler")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "AdgangTilFMKInformationForAktuellePatient", "Adgang til FMK information for aktuelle patient")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "AdgangTilE-JournalForAktuellePatient", "Adgang til e-Journal for aktuelle patient")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "SoegeEfterOgSeJornalerSomHarVaeretTilknyttetAktuelleBeredskabIndenForSeneste24Timer", "Søge efter og se jornaler som har været tilknyttet aktuelle beredskab inden for seneste 24 timer")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "SoegeEfterOgSeJournalerSomAktuelleBrugerHarRegistreretIndenForSenesteUge", "Søge efter og se journaler som aktuelle bruger har registreret inden for seneste uge")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "SoegeEfterOgSeJournalerUdfraPatientIdentifikationIndenForSenesteMaaned", "Søge efter og se journaler udfra patient identifikation inden for seneste måned")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Registreijournalerpaaaktivhaendelse", "Registre i journaler på aktiv hændelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Redigeretidligerejournaler", "Redigere tidligere journaler")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "RegistreringafmedicinklasseA", "Registrering af medicinklasse A")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "RegistreringafmedicinklasseB", "Registrering af medicinklasse B")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "RegistreringafmedicinklasseC", "Registrering af medicinklasse C")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Oprettebehandlingsplads", "Oprette behandlingsplads")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Visningafbehandlingspladstilknyttetaktuelhaendelse", "Visning af behandlingsplads tilknyttet aktuel hændelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Oprettepatienterpaaenbehandlingsplads", "Oprette patienter på en behandlingsplads")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/mobil", "Oprettejournalerpaaenhaendelseviamobilenhed", "Oprette journaler på en hændelse via mobil enhed")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "LoggepaaCentralPPJ", "Logge på Central PPJ")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "OpretteBeskederTilMobileenhederTilhoerendeEgenRegion", "Oprette beskeder til mobileenheder tilhørende egen region")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeAktiveJournaler-Nationalt", "Se aktive journaler - Nationalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeAktiveJournaler-Regionalt", "Se aktive journaler - Regionalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeAktiveJournaler-EgenModtagelse", "Se aktive journaler - Egen modtagelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeAktiveJournalerHvorDerIkkeErRegistreretModtagelse-Nationalt", "Se aktive journaler hvor der ikke er registreret modtagelse - Nationalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Seaktivejournalerhvorderikkeerregistreretmodtagelse-Regionalt", "Se aktive journaler hvor der ikke er registreret modtagelse - Regionalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SoegeEfterOgSeAfsluttedeJournalerUdfraPatientInformation(CPR/Navn)-Nationalt", "Søge efter og se afsluttede journaler udfra patient information (CPR/Navn)  - Nationalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SoegeEfterOgSeAfsluttedeJournalerUdfraPatientInformation(CPR/Navn)-Regionalt", "Søge efter og se afsluttede journaler udfra patient information (CPR/Navn)  - Regionalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Soegeefterogseafsluttedejournalerudfrapatientinformation(CPR/Navn)-Egenmodtagelse", "Søge efter og se afsluttede journaler udfra patient information (CPR/Navn) - Egen modtagelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Soegeefterogseafsluttedejournalerudfraopgaveinformation(Optagested-Adresseetc.)-Nationalt", "Søge efter og se afsluttede journaler udfra opgave information (Optagested - Adresse etc.)  - Nationalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Soegeefterogseafsluttedejournalerudfraopgaveinformation(Optagested-Adresseetc.)-Regionalt", "Søge efter og se afsluttede journaler udfra opgave information (Optagested - Adresse etc.) - Regionalt")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SoegeEfterOgSeAfsluttedeJournalerUdfraOpgaveInformation(Optagested-AdresseEtc.)-EgenModtagelse", "Søge efter og se afsluttede journaler udfra opgave information (Optagested - Adresse etc.) - Egen modtagelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Registrer'tilbagemeldingertiloperatoer'paajournalersommanharadgangtil", "Registrer 'tilbagemeldinger til operatør' på journaler som man har adgang til")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Sealle'tilbagemeldingertiloperatoer'paajournalersommanharadgangtil", "Se alle 'tilbagemeldinger til operatør' på journaler som man har adgang til")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Angivekapaciteterforegenmodtagelse", "Angive kapaciteter for egen modtagelse")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "AngiveKapaciteterForModtagelserIEgenRegion", "Angive kapaciteter for modtagelser i egen region")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeOgRedigereBehandlerpladslogForEgenRegion", "Se og redigere behandlerpladslog for egen region")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeBehandlerpladslogForEgenRegion", "Se behandlerpladslog for egen region")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeBehandlerpladslogForAndreRegioner", "Se behandlerpladslog for andre regioner")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "AdgangTilEgenRegionsStatistikWebInterface", "Adgang til egen regions statistik web interface")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "OpretteNyeStandardRapporterIEgenRegionsStatistikdatabase", "Oprette nye standard rapporter i egen regions statistikdatabase")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "AdgangTilEgenOperatoersStatistikWebInterface", "Adgang til egen operatørs statistik web interface")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "OpretteNyeStandardRapporterIEgenOperatoersStatistikdatabase", "Oprette nye standard rapporter i egen operatørs statistikdatabase")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "RedigereStamdata", "Redigere stamdata")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeLogFiler", "Se log filer")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Alarmopsaetning", "Alarmopsætning")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeOgSoegeEfterJournaler-Egne", "Se og søge efter journaler - Egne")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeOgSoegeEfterJournaler-Operatoer", "Se og søge efter journaler - Operatør")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeOgSoegeEfterJournaler-SupervisorFor", "Se og søge efter journaler - Supervisor for")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeKompetencebrug-Egen", "Se kompetencebrug - Egen")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeKompetencebrug-Operatoer", "Se kompetencebrug - Operatør")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeKompetencebrug-SupervisorFor", "Se kompetencebrug - Supervisor for")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SeTilbagemeldingerPaaJournalerSomBrugerenHarAdgangTil", "Se tilbagemeldinger på journaler som brugeren har adgang til")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "Sekommentartiljournalersombrugerenharadgangtil", "Se kommentar til journaler som brugeren har adgang til")]
        [InlineAutoConverteData("http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/central", "SkriveOgSeKommentarTilJournalerSomBrugerenHarAdgangTil", "Skrive og se kommentar til journaler som brugeren har adgang til")]
        public void PermissionShortNamesHaveDefaultValuesBasedOnTitle(
            string permissionUri,
            string permissionValue,
            string permissionTitle,
            ConvertToClaimRulesCommand sut
            )
        {
            sut.RoleAssignmentFile = ConvertToClaimRulesCommandTest.updatedLayoutFileWithRolesForAllPermissionsIncompleteShortnames;
            sut.PermissionsUriPrefix = "http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/ppj/";
            sut.PermissionIdsStartCell = new GridCoordinate(5, 0);
            sut.PermissionsShortNameColumnIndex = 1;
            sut.PermissionsTitleColumnIndex = 2;
            sut.RoleValuesStartCell = new GridCoordinate(2, 10);

            var claimRules = sut.Invoke<RolePermissionClaimRule>().ToList();
            var actual = claimRules
                .Where(cr => cr.Permission.AbsoluteUri == permissionUri)
                .Where(cr => cr.Title == permissionTitle)
                .Single()
                .Value;

            Assert.Equal(permissionValue, actual);
        }
    }
}

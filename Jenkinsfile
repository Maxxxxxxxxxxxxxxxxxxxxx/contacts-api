pipeline {
  agent any
  options {
    timeout(5)
    gitLabConnection('GitLab')
  }
  environment {
    DATE = new Date().format('yy.M')
    TAG = "${DATE}.${BUILD_NUMBER}"
  }
  triggers {
    gitlab(
      triggerOnPush: true,
      triggerOnMergeRequest: true,
      branchFilterType: 'All',
      addVoteOnMergeRequest: true
    )
  }
  
 stages {
        stage('Checkout') {
            steps {
                // Check out the source code from your version control system (e.g., Git)
                checkout scm
            }
        }

        stage('Restore Dependencies') {
            steps {
                // Restore NuGet packages for the ASP.NET project
                bat 'nuget restore YourSolution.sln'
            }
        }

        stage('Build') {
            steps {
                // Build the ASP.NET project
                bat 'msbuild /p:Configuration=Release YourProject.csproj'
            }
        }

        stage('Run Tests') {
            steps {
                // Run unit tests if applicable
                bat 'vstest.console.exe YourProject.Tests.dll'
            }
        }

        stage('Publish') {
            steps {
                // Publish the ASP.NET application
                bat 'dotnet publish -c Release -o .\\publish'
            }
        }

        stage('Deploy') {
            steps {
                // Add deployment steps if needed (e.g., deploy to a web server)
                // Example: Copy files to a server using SSH
                sh 'scp -r ./publish/* user@server:/path/to/destination'
            }
        }
    }
}
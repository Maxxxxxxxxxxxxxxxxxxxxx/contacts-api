pipeline {
    agent any

    options {
        timeout(5)
        gitLabConnection('GitLab')
    }
    
    triggers {
        gitlab(
            triggerOnPush: true,
            triggerOnMergeRequest: true,
            branchFilterType: 'All',
            addVoteOnMergeRequest: true)
    }

    stages {
        
        stage('build') {
            steps {
                script {
                    def dockerImage = docker.build("contactsApi", "-f ./Dockerfile .")
                }
            }
        }
    
        stage('Hello') {
            steps {
                echo 'Hello World'
            }
        }
    }
}
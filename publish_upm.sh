#!/bin/sh
# 获取当前分支
currentBranch=$(git symbolic-ref --short -q HEAD)

#单发模块
upmModule=("AndroidDependencyResolver")
githubRepoName=("android_dependency_resolver")
token=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

tag=$1
#是否正式发布，
publish2Release=$2

# 发布 UPM 脚本
publishUPM() {
    git config user.email "bot@xd.com"
    git config user.name "bot"

    git tag -d $(git tag)
    
    git branch -D github_upm
    
    git subtree split --prefix=Assets/TapTap/AndroidDependencyResolver --branch github_upm
    
    git remote rm "$1"
    
    # publish2Release=false
    # if [ $publish2Release=true ]; then
    #     echo "start push $1 to git@github.com:xd-platform/AndroidDependencyResolver.git"
    #     git remote add "$1" git@github.com:xd-platform/AndroidDependencyResolver.git
    # else
    #     echo "start push $1 to git@github.com:luckisnow/AndroidDependencyResolver.git"  
    #     git remote add "$1" https://$token@github.com/luckisnow/AndroidDependencyResolver.git
    # fi;

    echo "start push $1 to git@github.com:luckisnow/AndroidDependencyResolver.git"  
    git remote add "$1" https://$token@github.com:luckisnow/AndroidDependencyResolver.git
    
    git checkout github_upm --force
    
    git tag "$2"
    
    git fetch --unshallow github_upm
    
    git push "AndroidDependencyResolver" main --force --tags
    
    git checkout "$currentBranch" --force
        
}
for ((i=0;i<${#upmModule[@]};i++)); do
    publishUPM "${upmModule[$i]}" "$tag"
done
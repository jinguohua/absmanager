angular.module('cnabsSrvUser', [])
.service('userHelper', function () {
    //所有产品相关用户信息
    this.allUserInfo = null;
    this.getAllUserInfo = function () {
        return this.allUserInfo;
    }

    //所有授权用户
    this.allAuthedUsers = null;
    this.getAllAuthedUsers = function () {
        return this.allAuthedUsers;
    }

    //重新加载所有信息
    this.reload = function (projectSeriesGuid, onFinished) {
        var self = this;
        var param = { projectSeriesGuid: projectSeriesGuid };

        var isSuccessGetTeamMembers = false;
        var isSuccessGetAllAuthedUsers = false;
        cnabsAjax("获取项目成员", "/TeamMember/GetTeamMembers", param, function (data) {
            self.allUserInfo = data;
            isSuccessGetTeamMembers = true;
            if (isSuccessGetTeamMembers && isSuccessGetAllAuthedUsers) {
                isSuccessGetTeamMembers = false;
                isSuccessGetAllAuthedUsers = false;
                onFinished();
            }
        });

        cnabsAjax("获取成员列表", "/EnterpriseUser/GetAllAuthedUsers", null, function (data) {
            self.allAuthedUsers = [];
            $.each(data, function () {
                self.allAuthedUsers.push({
                    userName: this.userName,
                    fullName: cnabsFormatUserName(this),
                    realName: this.realName,
                });
            })
            isSuccessGetAllAuthedUsers = true;
            if (isSuccessGetTeamMembers && isSuccessGetAllAuthedUsers) {
                isSuccessGetTeamMembers = false;
                isSuccessGetAllAuthedUsers = false;
                onFinished();
            }
        });
    }

    //判断用户是否是项目成员
    this.isTeamMember = function (userName) {
        return _.map(this.allUserInfo.TeamMembers, 'UserName').indexOf(userName) != -1;
    }

    //判断用户是否是项目管理员
    this.isTeamAdmin = function (userName) {
        return _.map(this.allUserInfo.TeamAdmins, 'UserName').indexOf(userName) != -1;
    }

    //判断用户是否是项目负责人/创建者
    this.isChief = function (userName) {
        return userName == this.allUserInfo.PersonInCharge.UserName
            || userName == this.allUserInfo.Creator.UserName;
    }

    this.getUsersToAddAdmin = function () {
        var self = this;
        return _.filter(this.allAuthedUsers, function (user) {
            return !self.isChief(user.userName) && !self.isTeamAdmin(user.userName);
        });
    }

    this.convertUserOptionArray = function (userArray) {
        var userOptionArray = [];
        $.each(userArray, function () {
            userOptionArray.push([this.userName, this.fullName]);
        });
        return userOptionArray;
    }

    this.getUserToTeamMember = function () {
        var self = this;
        return _.filter(this.allAuthedUsers, function (user) {
            return !self.isChief(user.userName) && !self.isTeamAdmin(user.userName) && !self.isTeamMember(user.userName);
        });
    }
});

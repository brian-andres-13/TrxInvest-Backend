pragma solidity >=0.5.0 <=0.7.0;

contract TronInvestor {
    uint256 public allInvest;

    constructor() payable public{
        allInvest = 0;
    }
  
    function() external payable {}

    struct Investment {
        address investor;
        uint256 amount;
        uint256 date;
        uint256 secIncome; // 1 equals 0.00001(0.001%)
        uint256 dealSeconds;
    }

    struct WithdrawHistory {
        address investor;
        uint256 date;
        uint256 amount;
    }

    struct Referral {
        address invitee;
        address inviter;
        uint256 date;
    }

    struct UserInfo {
        address investor;
        uint256 referralReward;
        uint256 withdrawable;
        uint256 totalInvest;
        uint256 totalDividends;
    }

    mapping(address => Investment[]) public invests;
    mapping(address => WithdrawHistory[]) public withdraws;
    mapping(address => Referral[]) public referrals;

    function getAllTotalInvest() public view returns (uint256 ret)
    {
        return allInvest;
    }

    function getTotalInvest() public view returns (uint256 ret)
    {
        Investment[] memory userInvests = invests[msg.sender];
        uint256 totalInvest = 0;

        uint length = userInvests.length;
        for (uint i = 0 ; i < length; i ++) {
            totalInvest += userInvests[i].amount;
        }

        return totalInvest;
    }

    function getTotalIncome() public view returns (uint256 ret)
    {
        Investment[] memory userInvests = invests[msg.sender];
        uint256 totalIncome = 0;

        uint length = userInvests.length;
        for (uint i = 0 ; i < length; i ++) {
            uint dealSecs;
            if (userInvests[i].dealSeconds == 0 || block.timestamp < userInvests[i].date + userInvests[i].dealSeconds) {
                dealSecs = block.timestamp - userInvests[i].date;
            }
            else {
                dealSecs = userInvests[i].dealSeconds;
            }
            totalIncome += userInvests[i].amount * userInvests[i].secIncome * dealSecs / 100000000;
        }
        return totalIncome;
    }

    function getTotalWithdraw() public view returns (uint256 ret)
    {
        WithdrawHistory[] memory userWithdraws = withdraws[msg.sender];
        uint256 totalWithdraw = 0;
        uint length = userWithdraws.length;
        for (uint i = 0 ; i < length; i ++) {
            totalWithdraw += userWithdraws[i].amount;
        }
        return totalWithdraw;
    }

    function getTotalReferral() public view returns (uint256 ret)
    {
        Investment[] memory userInvests = invests[msg.sender];
        Referral[] memory userReferral = referrals[msg.sender];
        uint256 totalReferral = 0;

        uint length = userReferral.length;
        for (uint i = 0 ; i < length ; i ++) {
            uint ilen = userInvests.length;
            for (uint j = 0 ; j < ilen ; j ++) {
                if (userInvests[j].date < userReferral[i].date) {
                    totalReferral += userInvests[j].amount * 5 / 1000;
                }
            }
        }

        return totalReferral;
    }

    function getOneSecondIncome() public view returns (uint256 ret)
    {
        Investment[] memory userInvests = invests[msg.sender];
        uint256 secondIncome = 0;

        uint length = userInvests.length;
        for (uint i = 0 ; i < length; i ++) {
            if (userInvests[i].dealSeconds == 0 || block.timestamp < userInvests[i].date + userInvests[i].dealSeconds) {
                secondIncome += userInvests[i].amount * userInvests[i].secIncome / 100000000;
            }
        }
        return secondIncome;
    }

    function postInvest(
        uint256 amount,
        uint256 secIncome,
        uint256 dealSeconds
    ) public
    {
        require(amount > 10, "Invest amount is less than minimum.");

        invests[msg.sender].push(Investment({
            investor: msg.sender,
            amount: amount,
            date: block.timestamp,
            secIncome: secIncome,
            dealSeconds: dealSeconds
        }));
        allInvest = allInvest + amount;
    }

    function postCompound(
        uint256 amount,
        uint256 secIncome,
        uint256 dealSeconds
    ) public
    {
        require(amount > 10, "Invest amount is less than minimum.");
        require(invests[msg.sender].length > 0, "Withdraw is not allowed for non-invested users.");
        
        uint256 withdrawable = getTotalIncome() + getTotalReferral() - getTotalWithdraw();
        require(amount <= withdrawable, "Withdraw amount exceeds the maximum");
        require(amount > 0, "Withdraw amount is less than minimum");

        withdraws[msg.sender].push(WithdrawHistory({
            investor: msg.sender,
            date: block.timestamp,
            amount: amount
        }));

        invests[msg.sender].push(Investment({
            investor: msg.sender,
            amount: amount,
            date: block.timestamp,
            secIncome: secIncome,
            dealSeconds: dealSeconds
        }));
        allInvest = allInvest + amount;
    }

    function postReferral(
        address invitee
    ) public
    {
        require(invitee != msg.sender, "You cannot invite yourself.");

        uint length = referrals[invitee].length;
        for (uint i = 0 ; i < length ; i ++) {
            require(referrals[invitee][i].inviter != msg.sender, "The inviter already invited same invitee.");
        }

        referrals[invitee].push(Referral({
            invitee: invitee,
            inviter: msg.sender,
            date: block.timestamp
        }));
    }

    function postWithdraw(
        uint256 amount
    ) public payable
    {
        require(invests[msg.sender].length > 0, "Withdraw is not allowed for non-invested users.");
        
        uint256 withdrawable = getTotalIncome() + getTotalReferral() - getTotalWithdraw();
        require(amount <= withdrawable, "Withdraw amount exceeds the maximum");
        require(amount > 0, "Withdraw amount is less than minimum");

        msg.sender.transfer(amount);
        withdraws[msg.sender].push(WithdrawHistory({
            investor: msg.sender,
            date: block.timestamp,
            amount: amount
        }));
    }
}

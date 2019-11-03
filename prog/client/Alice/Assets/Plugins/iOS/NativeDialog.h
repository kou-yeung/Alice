//
//  NativeDialog.h
//  UnityLib
//
//  Created by mshirooka on 2014/03/04.
//  Copyright (c) 2014年 mshirooka. All rights reserved.
//

#import <Foundation/Foundation.h>

enum ButtonType {
    SubmitOnly,
    YesNo
};

enum ClickButton {
    SubmitOrNegative=0,
    Positive=1
};

@interface NativeDialog : NSObject<UIAlertViewDelegate> {
    NSString *labelPositive;
    NSString *labelNegative;
    UIAlertView *alertViewInstance;
}
-(id)init;
+ (NativeDialog*)getInstance;
- (void)showDialogWithTitle:(NSString*)title message:(NSString*)message buttonType:(int)buttonType;
- (void)showDialogWithMessage:(NSString*)message buttonType:(int)buttonType;
- (void)setButtonLabeleWithPositive:(NSString*)positive;
- (void)setButtonLabeleWithPositive:(NSString*)positive negative:(NSString*)negative;
- (void)dismiss;
@end

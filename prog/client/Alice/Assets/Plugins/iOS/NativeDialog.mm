//
//  NativeDialog.m
//  UnityLib
//
//  Created by mshirooka on 2014/03/04.
//  Copyright (c) 2014年 mshirooka. All rights reserved.
//

#import "NativeDialog.h"

extern void UnitySendMessage(const char *, const char *, const char *);

extern "C" {
    void DialogShowWithMessage(const char *message, int buttonType ) {
        [[NativeDialog getInstance]
                showDialogWithMessage:[NSString stringWithUTF8String:message]
                buttonType:buttonType];
    }
    
    void DialogShowWithTitle(const char *title, const char *message, int buttonType) {
        [[NativeDialog getInstance]
                showDialogWithTitle:
                    [NSString stringWithUTF8String:title]
                    message:[NSString stringWithUTF8String:message]
                    buttonType:buttonType];
    }
    
    void DialogDismiss(){
         [[NativeDialog getInstance]dismiss];
    }
    

    void DialogSetButtonLabel(const char *positive) {
        [[NativeDialog getInstance]
            setButtonLabeleWithPositive:[NSString stringWithUTF8String:positive]];
    }
    
    void DialogSetButtonLabels(const char *positive, const char *negative) {
        [[NativeDialog getInstance]
            setButtonLabeleWithPositive:[NSString stringWithUTF8String:positive]
                               negative:[NSString stringWithUTF8String:negative]];
    }
}


@implementation NativeDialog

- (id)init
{
    if (self = [super init]) {
        NSLog(@"init");
        labelNegative = @"No";
        labelPositive = @"Yes";
    }
    return self;
}

+(NativeDialog*) getInstance {
    static NativeDialog *instance;
    
    @synchronized(self) {
        if(instance == nil) {
            instance = [[self alloc]init];
        }
    }
    
    return instance;
}

-(void)showDialogWithTitle:(NSString*)title
                   message:(NSString*)message
                   buttonType:(int)buttonType {
    if( alertViewInstance != nil ) {
        return;
    }
    
    alertViewInstance = [[UIAlertView alloc] init];
    
    alertViewInstance.title = title;
    alertViewInstance.message = message;
    alertViewInstance.delegate = self;
    alertViewInstance.cancelButtonIndex = 0;
    switch( buttonType ) {
    case SubmitOnly:
        [alertViewInstance addButtonWithTitle:labelPositive];
        break;
    default:
        [alertViewInstance addButtonWithTitle:labelNegative];
        [alertViewInstance addButtonWithTitle:labelPositive];
        break;
    }

    [alertViewInstance show];
}

-(void)showDialogWithMessage:(NSString*)message
                    buttonType:(int)buttonType {
    
    if( alertViewInstance != nil ) {
        return;
    }
    
    alertViewInstance = [[UIAlertView alloc] init];
    
    alertViewInstance.title = nil;
    alertViewInstance.message = message;
    alertViewInstance.delegate = self;
    alertViewInstance.cancelButtonIndex = 0;
    switch( buttonType ) {
    case SubmitOnly:
        [alertViewInstance addButtonWithTitle:labelPositive];
        break;
    default:
        [alertViewInstance addButtonWithTitle:labelNegative];
        [alertViewInstance addButtonWithTitle:labelPositive];
        break;
    }
;

    [alertViewInstance show];
}

- (void)setButtonLabeleWithPositive:(NSString*)positive{
    [labelPositive release];

    labelPositive = [NSString stringWithString:positive];
    
    [labelPositive retain];
}

- (void)setButtonLabeleWithPositive:(NSString*)positive negative:(NSString*)negative {
    [labelPositive release];
    [labelNegative release];

    labelPositive = [NSString stringWithString:positive];
    labelNegative = [NSString stringWithString:negative];
    
    [labelPositive retain];
    [labelNegative retain];
}

- (void)dismiss {
    if( alertViewInstance == nil ) {
        return;
    }
    
    //NSLog(@"dismiss");
    
    [alertViewInstance dismissWithClickedButtonIndex:SubmitOrNegative animated:true];
    [alertViewInstance release];
    alertViewInstance = nil;
}

- (void)alertView:(UIAlertView*)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    
    if( alertView == nil ) {
        UnitySendMessage("PlatformDialog", "OnNegative", "negative");
        return;
    }
    
    switch (buttonIndex) {
    case SubmitOrNegative:
        if( [alertView numberOfButtons] == 1 ) {
            //NSLog(@"clicked ok");
            UnitySendMessage("PlatformDialog", "OnPositive", "positive");
        }
        else {
            //NSLog(@"clicked cancel");
            UnitySendMessage("PlatformDialog", "OnNegative", "negative");
        }
        break;
    case Positive:
        //NSLog(@"clicked ok");
        UnitySendMessage("PlatformDialog", "OnPositive", "positive");
        break;
    default:
        break;
    }
    
    [alertViewInstance release];
    alertViewInstance = nil;
}

@end


